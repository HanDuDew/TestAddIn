using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBeau.Inventor
{
    internal class DrawingRetrieveDims : InventorMethodBase
    {
        /// <summary>
        /// Display name for this method
        /// </summary>
        public override string DisplayName => "Retrieve Dimensions";

        /// <summary>
        /// Retrieves dimensions from a specific drawing view (updated to use passed drawing view)
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The drawing view to retrieve dimensions from</param>
        public void RetrieveDims(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            try
            {
                if (drawingView == null)
                {
                    throw new InvalidOperationException("Drawing view cannot be null.");
                }

                // Get the drawing document
                global::Inventor.DrawingDocument oDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;

                // Retrieve general dimensions for the provided view
                oDoc.ActiveSheet.DrawingDimensions.GeneralDimensions.Retrieve(drawingView);

                System.Diagnostics.Debug.WriteLine($"Successfully retrieved dimensions from drawing view: {drawingView.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error occurred while retrieving dimensions: {ex.Message}");
                throw new Exception($"Error occurred while retrieving dimensions: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves dimensions from a drawing view using Pick method (for standalone use)
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void RetrieveDimsWithPick(global::Inventor.Application inventorApp)
        {
            try
            {
                // Equivalent to: Dim oDoc As DrawingDocument / Set oDoc = ThisApplication.ActiveDocument
                global::Inventor.DrawingDocument oDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;

                // Equivalent to: Dim oView As DrawingView / Set oView = ThisApplication.CommandManager.Pick(...)
                global::Inventor.DrawingView oView = inventorApp.CommandManager.Pick(
                    global::Inventor.SelectionFilterEnum.kDrawingViewFilter, 
                    "Select a drawing view") as global::Inventor.DrawingView;

                if (oView == null)
                {
                    throw new InvalidOperationException("No drawing view was selected or selection was cancelled.");
                }

                // Use the new method that accepts a drawing view
                RetrieveDims(inventorApp, oView);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error occurred while retrieving dimensions: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Alternative method that works with pre-selected drawing view (for UI integration)
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void RetrieveDimensionsFromSelectedView(global::Inventor.Application inventorApp)
        {
            try
            {
                // Ensure we are in a drawing document
                if (inventorApp.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
                {
                    throw new InvalidOperationException("Please open a drawing document.");
                }

                global::Inventor.DrawingDocument oDrawDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;
                global::Inventor.Sheet oSheet = oDrawDoc.ActiveSheet;

                // Check if a drawing view is selected (following existing codebase pattern)
                if (oDrawDoc.SelectSet.Count == 0)
                {
                    throw new InvalidOperationException("No drawing view is selected. Please select a drawing view first.");
                }

                global::Inventor.DrawingView oDrawingView = (global::Inventor.DrawingView)oDrawDoc.SelectSet[1];

                // Retrieve general dimensions for the selected view
                oSheet.DrawingDimensions.GeneralDimensions.Retrieve(oDrawingView);

                System.Diagnostics.Debug.WriteLine($"Successfully retrieved dimensions from pre-selected drawing view: {oDrawingView.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving dimensions: {ex.Message}");
                throw new Exception($"Error retrieving dimensions: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Static method to retrieve dimensions using Pick method (direct VBA equivalent)
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public static void RetrieveDimsStatic(global::Inventor.Application inventorApp)
        {
            DrawingRetrieveDims retrieveDims = new DrawingRetrieveDims();
            retrieveDims.RetrieveDimsWithPick(inventorApp);
        }

        /// <summary>
        /// Implementation of the abstract Execute method from InventorMethodBase
        /// This provides a consistent interface for the QueuedInventorMethodsHelper
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The selected drawing view to operate on</param>
        public override void Execute(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            RetrieveDims(inventorApp, drawingView);
        }
    }
}
