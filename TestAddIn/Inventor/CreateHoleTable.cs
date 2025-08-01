using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBeau.Inventor
{
    internal class CreateHoleTable : InventorMethodBase
    {
        /// <summary>
        /// Display name for this method
        /// </summary>
        public override string DisplayName => "Add Hole Table";

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
                throw new Exception($"Error occured while creating hole tables: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates hole tables using the provided drawing view
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The drawing view to create hole tables for</param>
        public void CreateHoleTables(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            try
            {
                if (drawingView == null)
                {
                    throw new InvalidOperationException("Drawing view cannot be null.");
                }

                // Set a reference to the drawing document.
                global::Inventor.DrawingDocument oDrawDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;

                // Set a reference to the active sheet.
                global::Inventor.Sheet oActiveSheet = oDrawDoc.ActiveSheet;

                // Use the provided drawing view instead of getting from selection
                global::Inventor.DrawingView oDrawingView = drawingView;

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
                throw new Exception($"Error occurred while creating hole tables: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Legacy method for backward compatibility - uses selection set
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        public void CreateHoleTablesLegacy(global::Inventor.Application inventorApp)
        {
            try
            {
                // Set a reference to the drawing document.
                global::Inventor.DrawingDocument oDrawDoc = (global::Inventor.DrawingDocument)inventorApp.ActiveDocument;

                // Check if a drawing view is selected
                if (oDrawDoc.SelectSet.Count == 0)
                {
                    throw new InvalidOperationException("No drawing view is selected. Please select a drawing view first.");
                }

                global::Inventor.DrawingView oDrawingView = (global::Inventor.DrawingView)oDrawDoc.SelectSet[1];

                // Call the new method with the selected view
                CreateHoleTables(inventorApp, oDrawingView);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while creating hole tables: {ex.Message}", ex);
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

        /// <summary>
        /// Implementation of the abstract Execute method from InventorMethodBase
        /// This provides a consistent interface for the QueuedInventorMethodsHelper
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The selected drawing view to operate on</param>
        public override void Execute(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView)
        {
            CreateHoleTables(inventorApp, drawingView);
        }
    }
}