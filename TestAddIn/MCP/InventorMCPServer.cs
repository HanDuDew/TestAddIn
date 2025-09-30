using System;
using System.Collections.Generic;
using System.Linq;
using AutoBeau.Common;
using AutoBeau.Inventor;
using AutoBeau.Core;

namespace AutoBeau.MCP
{
    /// <summary>
    /// Lightweight MCP-style server wrapper that exposes Inventor methods as callable tools.
    /// (Does not depend on a concrete MCP runtime yet – designed so a protocol layer can be added later.
    /// </summary>
    internal class InventorMCPServer
    {
        private static readonly Lazy<InventorMCPServer> _instance = new Lazy<InventorMCPServer>(() => new InventorMCPServer());
        public static InventorMCPServer Instance { get { return _instance.Value; } }

        private global::Inventor.Application _inventorApp;
        private QueuedInventorMethodsHelper _queueHelper;
        private bool _initialized;
        private AutoBeauDockableWindow _ui;

        private InventorMCPServer() { }

        /// <summary>
        /// Initialize the server with Inventor application and (optionally) UI window for messaging.
        /// Safe to call multiple times.
        /// </summary>
        public void Initialize(global::Inventor.Application app, AutoBeauDockableWindow ui = null)
        {
            if (_initialized && app == _inventorApp) return;
            _inventorApp = app;
            _ui = ui;
            _queueHelper = new QueuedInventorMethodsHelper(app);
            _initialized = true;
            AddMessage("MCP server initialized.");
        }

        private void EnsureInitialized()
        {
            if (!_initialized || _inventorApp == null)
                throw new InvalidOperationException("InventorMCPServer not initialized.");
        }

        private void AddMessage(string msg)
        {
            try
            {
                if (_ui != null)
                {
                    _ui.AddSystemMessage(msg);
                }
                System.Diagnostics.Debug.WriteLine("[MCP] " + msg);
            }
            catch { }
        }

        /// <summary>
        /// Returns metadata for all available Inventor methods (simulating an MCP tool response)
        /// </summary>
        public IList<object> ListMethods()
        {
            EnsureInitialized();
            var list = new List<object>();
            foreach (var m in _queueHelper.GetMethodQueue())
            {
                list.Add(new
                {
                    name = m.Name,
                    priority = m.Priority,
                    selected = m.IsSelected
                });
            }
            return list;
        }

        /// <summary>
        /// Update selection flags for the queued helper (simulates a tool invocation)
        /// </summary>
        public object SetSelections(bool retrieveDims, bool autoArrange, bool holeTable, bool centermarks)
        {
            EnsureInitialized();
            _queueHelper.SetMethodSelections(retrieveDims, autoArrange, holeTable, centermarks);
            AddMessage(string.Format("Selections updated RD:{0} AA:{1} HT:{2} CM:{3}", retrieveDims, autoArrange, holeTable, centermarks));
            return new { ok = true };
        }

        /// <summary>
        /// Execute a single method by display name. If drawingViewName is null, first view on active sheet is used.
        /// </summary>
        public object ExecuteMethod(string methodName, string drawingViewName = null)
        {
            EnsureInitialized();
            try
            {
                // Validate drawing document
                if (_inventorApp.ActiveDocument == null || _inventorApp.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
                {
                    return new { ok = false, error = "Active document is not a drawing document." };
                }

                global::Inventor.DrawingDocument doc = (global::Inventor.DrawingDocument)_inventorApp.ActiveDocument;
                global::Inventor.Sheet sheet = doc.ActiveSheet;
                global::Inventor.DrawingView view = null;

                if (!string.IsNullOrEmpty(drawingViewName))
                {
                    foreach (global::Inventor.DrawingView v in sheet.DrawingViews)
                    {
                        if (string.Equals(v.Name, drawingViewName, StringComparison.OrdinalIgnoreCase))
                        {
                            view = v; break;
                        }
                    }
                    if (view == null)
                        return new { ok = false, error = "Drawing view '" + drawingViewName + "' not found." };
                }
                else
                {
                    if (sheet.DrawingViews.Count > 0)
                        view = sheet.DrawingViews[1]; // 1-based COM collection
                }

                if (view == null)
                    return new { ok = false, error = "No drawing view available to execute method." };

                var methodItem = _queueHelper.GetMethodQueue().FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase));
                if (methodItem == null)
                    return new { ok = false, error = "Method '" + methodName + "' not found." };

                var result = InvokeInternal(methodItem, view);
                AddMessage(string.Format("Executed method '{0}' => {1}", methodName, result.Success ? "success" : "failure"));
                return new
                {
                    ok = result.Success,
                    method = methodName,
                    message = result.Message,
                    warning = result.IsWarning,
                    error = (!result.Success && !result.IsWarning) ? result.Message : null
                };
            }
            catch (Exception ex)
            {
                return new { ok = false, error = ex.Message };
            }
        }

        /// <summary>
        /// Executes all currently selected methods using the first drawing view (no interactive pick).
        /// </summary>
        public object ExecuteSelectedQueue()
        {
            EnsureInitialized();
            try
            {
                if (_inventorApp.ActiveDocument == null || _inventorApp.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
                {
                    return new { ok = false, error = "Active document is not a drawing document." };
                }

                global::Inventor.DrawingDocument doc = (global::Inventor.DrawingDocument)_inventorApp.ActiveDocument;
                global::Inventor.Sheet sheet = doc.ActiveSheet;
                if (sheet.DrawingViews.Count == 0)
                    return new { ok = false, error = "No drawing views on active sheet." };

                global::Inventor.DrawingView view = sheet.DrawingViews[1];

                // Filter selected
                var selected = new List<QueuedInventorMethodsHelper.InventorMethodItem>();
                foreach (var m in _queueHelper.GetMethodQueue())
                {
                    if (m.IsSelected) selected.Add(m);
                }
                if (selected.Count == 0)
                    return new { ok = false, error = "No methods selected." };

                var results = new List<object>();
                bool anyError = false;
                foreach (var m in selected.OrderBy(x => x.Priority))
                {
                    var r = InvokeInternal(m, view);
                    if (!r.Success && r.IsCritical)
                    {
                        anyError = true;
                        results.Add(new { method = m.Name, success = false, error = r.Message, critical = true });
                        break;
                    }
                    results.Add(new { method = m.Name, success = r.Success, message = r.Message, warning = r.IsWarning });
                    if (!r.Success) anyError = true;
                }

                AddMessage(string.Format("Executed {0} selected methods (errors: {1})", selected.Count, anyError));
                return new { ok = !anyError, results = results };
            }
            catch (Exception ex)
            {
                return new { ok = false, error = ex.Message };
            }
        }

        private MethodExecutionResult InvokeInternal(QueuedInventorMethodsHelper.InventorMethodItem item, global::Inventor.DrawingView view)
        {
            // Reuse logic similar to helper (non-public call path) to avoid re-picking views
            try
            {
                if (item.MethodInstance != null)
                {
                    item.MethodInstance.Execute(_inventorApp, view);
                    return new MethodExecutionResult
                    {
                        MethodName = item.Name,
                        Success = true,
                        Message = string.Format("{0} executed successfully on view '{1}'", item.Name, view.Name)
                    };
                }
                return new MethodExecutionResult
                {
                    MethodName = item.Name,
                    Success = false,
                    Message = "No implementation instance available.",
                    IsWarning = true
                };
            }
            catch (Exception ex)
            {
                return new MethodExecutionResult
                {
                    MethodName = item.Name,
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
