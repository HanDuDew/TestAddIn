using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBeau.Inventor
{
    internal class AutoAddCenterlinesAndCentermarks : InventorMethodBase
    {
        /// <summary>
        /// Display name for this method
        /// </summary>
        public override string DisplayName => "Auto Add Centerlines and Centermarks";

        /// <summary>
        /// Adds centerlines and centermarks to the drawing view
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The drawing view to work with</param>
        public void AddCenterlinesAndCentermarks(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            try
            {
                // Make sure we are in a drawing document
                if (inventorApp.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
                {
                    throw new InvalidOperationException("Please open a drawing document.");
                }
                if (drawingView == null)
                {
                    throw new InvalidOperationException("Drawing view cannot be null.");
                }
                
                global::Inventor.DrawingView oDrawingView = drawingView;

                AutomatedCenterlineSettings settings;

                drawingView.GetAutomatedCenterlineSettings(out settings);

                settings.ProjectionNormalAxis = true;
                settings.ProjectionParallelAxis = true;

                // Set a reference to the output
                ObjectsEnumerator results;
                // Set the automated centerline settings
                results = oDrawingView.SetAutomatedCenterlineSettings(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return;
            }
        }

        public override void Execute(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            AddCenterlinesAndCentermarks(inventorApp, drawingView);
        }
    }
}
