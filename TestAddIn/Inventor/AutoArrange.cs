using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvAddIn.Inventor
{
    internal class AutoArrange
    {
        /// <summary>
        /// Arranges all general dimensions automatically in the active drawing document
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void ArrangeAllGeneralDimensions(global::Inventor.Application inventorApp)
        {
            try
            {
                // Make sure we are in a drawing document
                if (inventorApp.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
                {
                    throw new InvalidOperationException("Please open a drawing document.");
                }

                global::Inventor.DrawingDocument oDrawDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;
                global::Inventor.Sheet oSheet = oDrawDoc.ActiveSheet;

                // Check if a drawing view is selected
                if (oDrawDoc.SelectSet.Count == 0)
                {
                    throw new InvalidOperationException("No drawing view is selected. Please select a drawing view first.");
                }

                global::Inventor.DrawingView oDrawingView = (global::Inventor.DrawingView)oDrawDoc.SelectSet[1];

                // Create a collection to hold dimensions to arrange
                global::Inventor.ObjectCollection oDimsToArrange = inventorApp.TransientObjects.CreateObjectCollection();

                // Iterate through all general dimensions on the sheet
                foreach (global::Inventor.GeneralDimension oGeneralDim in oSheet.DrawingDimensions.GeneralDimensions)
                {
                    try
                    {
                        // Show all extension lines for the dimension
                        oGeneralDim.ShowAllExtensionLines();
                        
                        // Try to promote to sketch and get anchor points (this might fail for some dimensions)
                        try
                        {
                            var sketchDim = oGeneralDim.PromoteToSketch();
                            if (sketchDim != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"Dimension promoted to sketch successfully");
                            }
                        }
                        catch
                        {
                            // Ignore errors for individual dimensions that can't be promoted
                        }

                        // Add the dimension to the collection
                        oDimsToArrange.Add(oGeneralDim);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with other dimensions
                        System.Diagnostics.Debug.WriteLine($"Error processing dimension: {ex.Message}");
                        continue;
                    }
                }

                if (oDimsToArrange.Count == 0)
                {
                    throw new InvalidOperationException("No individual general dimensions found to arrange.");
                }

                // Arrange the dimensions automatically
                try
                {
                    // Try to get the origin intent if available
                    if (oDrawingView.HasOriginIndicator)
                    {
                        global::Inventor.GeometryIntent oOriginIntent = oDrawingView.OriginIndicator.Intent;
                        
                        // Try arranging with origin intent first (some versions support this)
                        try
                        {
                            oSheet.DrawingDimensions.Arrange(oDimsToArrange, oOriginIntent);
                        }
                        catch
                        {
                            // Fall back to arranging without the origin intent parameter
                            oSheet.DrawingDimensions.Arrange(oDimsToArrange);
                        }
                    }
                    else
                    {
                        // Arrange without origin intent
                        oSheet.DrawingDimensions.Arrange(oDimsToArrange);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error arranging dimensions: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur
                throw new Exception($"Error in auto arrange functionality: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Static method to arrange all general dimensions
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public static void ArrangeAllGeneralDimensionsStatic(global::Inventor.Application inventorApp)
        {
            AutoArrange autoArrange = new AutoArrange();
            autoArrange.ArrangeAllGeneralDimensions(inventorApp);
        }
    }
}