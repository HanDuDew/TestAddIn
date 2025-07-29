using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvAddIn.Inventor
{
    internal class CreateHoleTable
    {
        /// <summary>
        /// Creates hole tables in the active drawing document
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void CreateHoleTables(global::Inventor.Application inventorApp)
        {
            try
            {
                // Set a reference to the drawing document.
                // This assumes a drawing document is active.
                global::Inventor.DrawingDocument oDrawDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;

                // Set a reference to the active sheet.
                global::Inventor.Sheet oActiveSheet = oDrawDoc.ActiveSheet;

                // Set a reference to the drawing view.
                // This assumes that a drawing view is selected.
                if (oDrawDoc.SelectSet.Count == 0)
                {
                    throw new InvalidOperationException("No drawing view is selected. Please select a drawing view first.");
                }

                global::Inventor.DrawingView oDrawingView = (global::Inventor.DrawingView)oDrawDoc.SelectSet[1];

                // Create origin indicator if it has not been already created.
                if (!oDrawingView.HasOriginIndicator)
                {
                    // Create point intent to anchor the origin to.
                    global::Inventor.GeometryIntent oDimIntent;
                    global::Inventor.Point2d oPointIntent;

                    // Get the first curve on the view
                    if (oDrawingView.DrawingCurves.Count > 0)
                    {
                        global::Inventor.DrawingCurve oCurve = null;
                        foreach (global::Inventor.DrawingCurve curve in oDrawingView.DrawingCurves)
                        {
                            oCurve = curve;
                            break; // Get the first curve
                        }

                        if (oCurve != null)
                        {
                            // Check if it has a start point
                            oPointIntent = oCurve.StartPoint;

                            if (oPointIntent == null)
                            {
                                // Else use the center point
                                oPointIntent = oCurve.CenterPoint;
                            }

                            oDimIntent = oActiveSheet.CreateGeometryIntent(oCurve, oPointIntent);

                            oDrawingView.CreateOriginIndicator(oDimIntent);
                        }
                    }
                }

                global::Inventor.Point2d oPlacementPoint;

                // Set a reference to the sheet's border
                global::Inventor.Border oBorder = oActiveSheet.Border;

                if (oBorder != null)
                {
                    // A border exists. The placement point
                    // is the top-left corner of the border.
                    oPlacementPoint = inventorApp.TransientGeometry.CreatePoint2d(
                        oBorder.RangeBox.MinPoint.X, 
                        oBorder.RangeBox.MaxPoint.Y);
                }
                else
                {
                    // There is no border. The placement point
                    // is the top-left corner of the sheet.
                    oPlacementPoint = inventorApp.TransientGeometry.CreatePoint2d(0, oActiveSheet.Height);
                }

                // Create a 'view' hole table
                // This hole table includes all holes as specified by the active hole table style
                global::Inventor.HoleTable oViewHoleTable = oActiveSheet.HoleTables.Add(oDrawingView, oPlacementPoint);

                oPlacementPoint.X = oActiveSheet.Width / 2;

                // Create a 'feature type' hole table
                // This hole table includes specified hole types only
                // Uncomment the following lines if you want to create a feature-type hole table
                /*
                global::Inventor.HoleTable oFeatureHoleTable = oActiveSheet.HoleTables.AddByFeatureType(
                    oDrawingView, 
                    oPlacementPoint,
                    true,   // Include simple holes
                    true,   // Include counterbore holes
                    true,   // Include countersink holes
                    true,   // Include tapped holes
                    false,  // Include clearance holes
                    false,  // Include spot face holes
                    false   // Include other holes
                );
                */
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur
                throw new Exception($"Error creating hole tables: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Overloaded method that accepts an Inventor Application object and creates hole tables
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public static void CreateHoleTablesStatic(global::Inventor.Application inventorApp)
        {
            CreateHoleTable holeTableCreator = new CreateHoleTable();
            holeTableCreator.CreateHoleTables(inventorApp);
        }
    }
}