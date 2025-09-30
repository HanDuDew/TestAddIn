using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AutoBeau.Services
{
    /// <summary>
    /// Provides a snapshot of the active Inventor session so the AI chat service
    /// can reason about the user’s current context.
    /// </summary>
    internal sealed class InventorContextService
    {
        private static readonly Lazy<InventorContextService> _instance =
            new Lazy<InventorContextService>(() => new InventorContextService());

        public static InventorContextService Instance => _instance.Value;

        private readonly object _syncRoot = new object();
        private global::Inventor.Application _inventorApplication;
        private string _uiMode = "Manual";
        private MethodSelectionsSnapshot _methodSelections = MethodSelectionsSnapshot.Empty;

        private InventorContextService()
        {
        }

        public void Initialize(global::Inventor.Application inventorApplication)
        {
            if (inventorApplication == null)
            {
                return;
            }

            lock (_syncRoot)
            {
                _inventorApplication = inventorApplication;
            }
        }

        public void UpdateUIMode(bool isAiMode)
        {
            lock (_syncRoot)
            {
                _uiMode = isAiMode ? "AI Chat" : "Manual";
            }
        }

        public void UpdateMethodSelections(bool retrieveDimensions, bool autoArrange, bool holeTable, bool centermarks)
        {
            lock (_syncRoot)
            {
                _methodSelections = new MethodSelectionsSnapshot(retrieveDimensions, autoArrange, holeTable, centermarks);
            }
        }

        public ContextSnapshot CaptureSnapshot()
        {
            global::Inventor.Application app;
            MethodSelectionsSnapshot selections;
            string uiMode;

            lock (_syncRoot)
            {
                app = _inventorApplication;
                selections = _methodSelections;
                uiMode = _uiMode;
            }

            var snapshot = new ContextSnapshot
            {
                MethodSelections = selections,
                UIMode = uiMode
            };

            if (app == null)
            {
                return snapshot;
            }

            try
            {
                var document = app.ActiveDocument;
                if (document == null)
                {
                    return snapshot;
                }

                snapshot.DocumentDisplayName = SafeGet(() => document.DisplayName);
                snapshot.DocumentFullFileName = SafeGet(() => document.FullFileName);
                snapshot.DocumentType = document.DocumentType.ToString();
                snapshot.IsDocumentDirty = document.Dirty;
                snapshot.ActiveEnvironment = SafeGet(() =>
                    app.UserInterfaceManager?.ActiveEnvironment?.DisplayName ??
                    app.UserInterfaceManager?.ActiveEnvironment?.InternalName);

                switch (document.DocumentType)
                {
                    case global::Inventor.DocumentTypeEnum.kDrawingDocumentObject:
                        snapshot.Drawing = CaptureDrawingContext((global::Inventor.DrawingDocument)document);
                        break;
                    case global::Inventor.DocumentTypeEnum.kPartDocumentObject:
                        snapshot.Part = CapturePartContext((global::Inventor.PartDocument)document);
                        break;
                    case global::Inventor.DocumentTypeEnum.kAssemblyDocumentObject:
                        snapshot.Assembly = CaptureAssemblyContext((global::Inventor.AssemblyDocument)document);
                        break;
                }

                snapshot.Selection = CaptureSelectionSummary(document);
            }
            catch (Exception ex)
            {
                snapshot.Errors.Add("Context capture failed: " + ex.Message);
            }

            return snapshot;
        }

        public string GetContextSummary()
        {
            var snapshot = CaptureSnapshot();
            return snapshot.ToSummaryString();
        }

        private static DrawingContext CaptureDrawingContext(global::Inventor.DrawingDocument document)
        {
            var ctx = new DrawingContext();

            try
            {
                ctx.TotalSheetCount = document.Sheets?.Count ?? 0;
                var activeSheet = document.ActiveSheet;
                if (activeSheet != null)
                {
                    ctx.ActiveSheetName = SafeGet(() => activeSheet.Name);
                    ctx.ActiveSheetSize = FormatSheetSize(activeSheet);
                    ctx.TotalViewCount = activeSheet.DrawingViews?.Count ?? 0;

                    foreach (global::Inventor.DrawingView view in activeSheet.DrawingViews)
                    {
                        ctx.Views.Add(BuildViewSummary(view));
                    }
                }
            }
            catch (Exception ex)
            {
                ctx.Errors.Add(ex.Message);
            }

            return ctx;
        }

        private static PartContext CapturePartContext(global::Inventor.PartDocument document)
        {
            var ctx = new PartContext();

            try
            {
                var definition = document.ComponentDefinition;
                ctx.SurfaceBodyCount = definition?.SurfaceBodies?.Count ?? 0;
                ctx.ParameterCount = definition?.Parameters?.Count ?? 0;

                try
                {
                    ctx.FeatureCount = definition?.Features?.Count ?? 0;
                }
                catch (Exception featureEx)
                {
                    ctx.Errors.Add("Features: " + featureEx.Message);
                }

                ctx.MaterialName = SafeGet(() => definition?.Material?.Name);

                try
                {
                    var massProps = definition?.MassProperties;
                    if (massProps != null)
                    {
                        ctx.Mass = massProps.Mass;
                        ctx.Volume = massProps.Volume;
                    }
                }
                catch (Exception massEx)
                {
                    ctx.Errors.Add("Mass properties: " + massEx.Message);
                }
            }
            catch (Exception ex)
            {
                ctx.Errors.Add(ex.Message);
            }

            return ctx;
        }

        private static AssemblyContext CaptureAssemblyContext(global::Inventor.AssemblyDocument document)
        {
            var ctx = new AssemblyContext();

            try
            {
                var definition = document.ComponentDefinition;
                ctx.OccurrenceCount = definition?.Occurrences?.Count ?? 0;
                ctx.ConstraintCount = definition?.Constraints?.Count ?? 0;

                try
                {
                    int grounded = 0;
                    int suppressed = 0;
                    if (definition?.Occurrences != null)
                    {
                        foreach (global::Inventor.ComponentOccurrence occurrence in definition.Occurrences)
                        {
                            if (occurrence.Grounded)
                            {
                                grounded++;
                            }

                            if (occurrence.Suppressed)
                            {
                                suppressed++;
                            }
                        }
                    }

                    ctx.GroundedOccurrenceCount = grounded;
                    ctx.SuppressedOccurrenceCount = suppressed;
                }
                catch (Exception occEx)
                {
                    ctx.Errors.Add("Occurrences: " + occEx.Message);
                }

                try
                {
                    var repsManager = document.ComponentDefinition?.RepresentationsManager;
                    if (repsManager != null)
                    {
                        string designViewName = string.Empty;
                        try
                        {
                            var activeView = repsManager.ActiveDesignViewRepresentation;
                            if (activeView != null)
                            {
                                designViewName = SafeGet(() => activeView.Name);
                            }
                        }
                        catch (Exception activeViewEx)
                        {
                            ctx.Errors.Add("Active design view: " + activeViewEx.Message);
                        }

                        ctx.ActiveDesignView = designViewName;
                    }
                }
                catch (Exception repsEx)
                {
                    ctx.Errors.Add("Design views: " + repsEx.Message);
                }
            }
            catch (Exception ex)
            {
                ctx.Errors.Add(ex.Message);
            }

            return ctx;
        }

        private static IReadOnlyList<string> CaptureSelectionSummary(global::Inventor._Document document)
        {
            var result = new List<string>();

            try
            {
                var selectSet = document.SelectSet;
                if (selectSet != null)
                {
                    int count = selectSet.Count;
                    for (int index = 1; index <= count; index++)
                    {
                        object selectedObject = selectSet[index];
                        result.Add(DescribeSelectedObject(selectedObject));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Add("Selection capture error: " + ex.Message);
            }

            return result;
        }

        private static string DescribeSelectedObject(object obj)
        {
            if (obj == null)
            {
                return "Unknown selection";
            }

            try
            {
                switch (obj)
                {
                    case global::Inventor.DrawingView view:
                        return $"Drawing view '{SafeGet(() => view.Name)}' (Scale {SafeGet(() => view.ScaleString)})";
                    case global::Inventor.Sheet sheet:
                        return $"Sheet '{SafeGet(() => sheet.Name)}'";
                    case global::Inventor.PartFeature feature:
                        return $"Feature '{SafeGet(() => feature.Name)}'";
                    case global::Inventor.ComponentOccurrence occurrence:
                        return $"Occurrence '{SafeGet(() => occurrence.Name)}'";
                    default:
                        return obj.GetType().Name;
                }
            }
            catch
            {
                return obj.GetType().Name;
            }
        }

        private static string BuildViewSummary(global::Inventor.DrawingView view)
        {
            if (view == null)
            {
                return "Drawing view";
            }

            try
            {
                var parts = new List<string>();
                var scale = SafeGet(() => view.ScaleString);
                if (!string.IsNullOrEmpty(scale))
                {
                    parts.Add("Scale " + scale);
                }

                var orientation = SafeGet(() => view.ViewType.ToString());
                if (!string.IsNullOrEmpty(orientation))
                {
                    parts.Add("Type " + orientation);
                }

                var modelName = SafeGet(() =>
                    view.ReferencedDocumentDescriptor?.DisplayName ??
                    view.ReferencedDocumentDescriptor?.FullDocumentName);
                if (!string.IsNullOrEmpty(modelName))
                {
                    parts.Add("Model " + modelName);
                }

                var name = SafeGet(() => view.Name);
                if (parts.Count == 0)
                {
                    return string.IsNullOrEmpty(name) ? "Drawing view" : name;
                }

                return string.IsNullOrEmpty(name)
                    ? string.Join(", ", parts)
                    : $"{name}: {string.Join(", ", parts)}";
            }
            catch
            {
                return SafeGet(() => view.Name) ?? "Drawing view";
            }
        }

        private static string FormatSheetSize(global::Inventor.Sheet sheet)
        {
            try
            {
                double width = sheet.Width;
                double height = sheet.Height;
                return string.Format(CultureInfo.InvariantCulture, "{0:0.##} x {1:0.##} (sheet units)", width, height);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SafeGet(Func<string> getter)
        {
            try
            {
                return getter() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal sealed class ContextSnapshot
        {
            public string DocumentDisplayName { get; set; } = string.Empty;
            public string DocumentFullFileName { get; set; } = string.Empty;
            public string DocumentType { get; set; } = string.Empty;
            public bool? IsDocumentDirty { get; set; }
            public string ActiveEnvironment { get; set; } = string.Empty;
            public string UIMode { get; set; } = string.Empty;
            public MethodSelectionsSnapshot MethodSelections { get; set; } = MethodSelectionsSnapshot.Empty;
            public DrawingContext Drawing { get; set; }
            public PartContext Part { get; set; }
            public AssemblyContext Assembly { get; set; }
            public IReadOnlyList<string> Selection { get; set; } = Array.Empty<string>();
            public List<string> Errors { get; } = new List<string>();

            public string ToSummaryString()
            {
                var sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(DocumentDisplayName))
                {
                    sb.AppendLine($"Document: {DocumentDisplayName} ({DocumentType})");
                }
                else if (!string.IsNullOrWhiteSpace(DocumentType))
                {
                    sb.AppendLine("Document type: " + DocumentType);
                }

                if (!string.IsNullOrWhiteSpace(DocumentFullFileName))
                {
                    sb.AppendLine("Path: " + DocumentFullFileName);
                }

                if (IsDocumentDirty.HasValue)
                {
                    sb.AppendLine("Unsaved changes: " + (IsDocumentDirty.Value ? "Yes" : "No"));
                }

                if (!string.IsNullOrWhiteSpace(ActiveEnvironment))
                {
                    sb.AppendLine("Inventor environment: " + ActiveEnvironment);
                }

                if (!string.IsNullOrWhiteSpace(UIMode))
                {
                    sb.AppendLine("AutoBeau mode: " + UIMode);
                }

                if (MethodSelections.HasAnySelection)
                {
                    sb.AppendLine("AutoBeau selections: " + MethodSelections.ToInlineText());
                }

                if (Drawing != null)
                {
                    var drawingSummary = Drawing.ToSummaryString();
                    if (!string.IsNullOrWhiteSpace(drawingSummary))
                    {
                        sb.AppendLine("Drawing info:");
                        sb.AppendLine(drawingSummary);
                    }
                }

                if (Part != null)
                {
                    var partSummary = Part.ToSummaryString();
                    if (!string.IsNullOrWhiteSpace(partSummary))
                    {
                        sb.AppendLine("Part info:");
                        sb.AppendLine(partSummary);
                    }
                }

                if (Assembly != null)
                {
                    var assemblySummary = Assembly.ToSummaryString();
                    if (!string.IsNullOrWhiteSpace(assemblySummary))
                    {
                        sb.AppendLine("Assembly info:");
                        sb.AppendLine(assemblySummary);
                    }
                }

                if (Selection != null && Selection.Count > 0)
                {
                    sb.AppendLine("Current selection:");
                    foreach (var item in Selection.Take(5))
                    {
                        sb.AppendLine(" - " + item);
                    }

                    if (Selection.Count > 5)
                    {
                        sb.AppendLine($" - ... ({Selection.Count - 5} more)");
                    }
                }

                if (Errors.Count > 0)
                {
                    sb.AppendLine("Context warnings: " + string.Join("; ", Errors));
                }

                return sb.ToString().Trim();
            }
        }

        internal sealed class DrawingContext
        {
            public string ActiveSheetName { get; set; } = string.Empty;
            public string ActiveSheetSize { get; set; } = string.Empty;
            public int TotalSheetCount { get; set; }
            public int TotalViewCount { get; set; }
            public List<string> Views { get; } = new List<string>();
            public List<string> Errors { get; } = new List<string>();

            public string ToSummaryString()
            {
                var sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(ActiveSheetName))
                {
                    var size = string.IsNullOrWhiteSpace(ActiveSheetSize) ? string.Empty : $" ({ActiveSheetSize})";
                    sb.AppendLine($" - Active sheet: {ActiveSheetName}{size}");
                }
                else if (!string.IsNullOrWhiteSpace(ActiveSheetSize))
                {
                    sb.AppendLine($" - Active sheet size: {ActiveSheetSize}");
                }

                if (TotalSheetCount > 0)
                {
                    sb.AppendLine($" - Total sheets: {TotalSheetCount}");
                }

                if (TotalViewCount > 0)
                {
                    sb.AppendLine($" - Views on active sheet: {TotalViewCount}");
                }

                if (Views.Count > 0)
                {
                    sb.AppendLine(" - View details:");
                    foreach (var view in Views.Take(5))
                    {
                        sb.AppendLine("   • " + view);
                    }

                    if (Views.Count > 5)
                    {
                        sb.AppendLine($"   • ... ({Views.Count - 5} more)");
                    }
                }

                if (Errors.Count > 0)
                {
                    sb.AppendLine(" - Issues: " + string.Join("; ", Errors));
                }

                return sb.ToString().TrimEnd();
            }
        }

        internal sealed class PartContext
        {
            public int SurfaceBodyCount { get; set; }
            public int ParameterCount { get; set; }
            public int FeatureCount { get; set; }
            public string MaterialName { get; set; } = string.Empty;
            public double? Mass { get; set; }
            public double? Volume { get; set; }
            public List<string> Errors { get; } = new List<string>();

            public string ToSummaryString()
            {
                var sb = new StringBuilder();

                sb.AppendLine($" - Surface/solid bodies: {SurfaceBodyCount}");
                sb.AppendLine($" - Parameters: {ParameterCount}");

                if (FeatureCount > 0)
                {
                    sb.AppendLine($" - Features: {FeatureCount}");
                }

                if (!string.IsNullOrWhiteSpace(MaterialName))
                {
                    sb.AppendLine(" - Material: " + MaterialName);
                }

                if (Mass.HasValue)
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, " - Mass (document units): {0:0.###}", Mass.Value));
                }

                if (Volume.HasValue)
                {
                    sb.AppendLine(string.Format(CultureInfo.InvariantCulture, " - Volume (document units): {0:0.###}", Volume.Value));
                }

                if (Errors.Count > 0)
                {
                    sb.AppendLine(" - Issues: " + string.Join("; ", Errors));
                }

                return sb.ToString().TrimEnd();
            }
        }

        internal sealed class AssemblyContext
        {
            public int OccurrenceCount { get; set; }
            public int ConstraintCount { get; set; }
            public int GroundedOccurrenceCount { get; set; }
            public int SuppressedOccurrenceCount { get; set; }
            public string ActiveDesignView { get; set; } = string.Empty;
            public List<string> Errors { get; } = new List<string>();

            public string ToSummaryString()
            {
                var sb = new StringBuilder();

                sb.AppendLine($" - Component occurrences: {OccurrenceCount}");
                sb.AppendLine($" - Constraints: {ConstraintCount}");

                if (GroundedOccurrenceCount > 0)
                {
                    sb.AppendLine($" - Grounded occurrences: {GroundedOccurrenceCount}");
                }

                if (SuppressedOccurrenceCount > 0)
                {
                    sb.AppendLine($" - Suppressed occurrences: {SuppressedOccurrenceCount}");
                }

                if (!string.IsNullOrWhiteSpace(ActiveDesignView))
                {
                    sb.AppendLine(" - Active design view: " + ActiveDesignView);
                }

                if (Errors.Count > 0)
                {
                    sb.AppendLine(" - Issues: " + string.Join("; ", Errors));
                }

                return sb.ToString().TrimEnd();
            }
        }

        internal readonly struct MethodSelectionsSnapshot
        {
            public MethodSelectionsSnapshot(bool retrieveDimensions, bool autoArrange, bool holeTable, bool centermarks)
            {
                RetrieveDimensions = retrieveDimensions;
                AutoArrange = autoArrange;
                HoleTable = holeTable;
                Centermarks = centermarks;
            }

            public bool RetrieveDimensions { get; }
            public bool AutoArrange { get; }
            public bool HoleTable { get; }
            public bool Centermarks { get; }

            public static MethodSelectionsSnapshot Empty => new MethodSelectionsSnapshot(false, false, false, false);

            public bool HasAnySelection => RetrieveDimensions || AutoArrange || HoleTable || Centermarks;

            public string ToInlineText()
            {
                string Format(string name, bool value) => $"{name}={(value ? "on" : "off")}";

                return string.Join(", ", new[]
                {
                    Format("Retrieve Dimensions", RetrieveDimensions),
                    Format("Auto Arrange", AutoArrange),
                    Format("Hole Table", HoleTable),
                    Format("Centerlines", Centermarks)
                });
            }
        }
    }
}
