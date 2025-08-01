using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBeau.Inventor;

namespace AutoBeau.Common
{
    /// <summary>
    /// Helper class to manage and execute Inventor methods in priority order
    /// Now manages all priorities centrally for better maintainability
    /// </summary>
    internal class QueuedInventorMethodsHelper
    {
        /// <summary>
        /// Class to represent an Inventor method with its priority and execution info
        /// </summary>
        public class InventorMethodItem
        {
            public string Name { get; set; }
            public int Priority { get; set; }
            public Action<global::Inventor.Application> ExecuteAction { get; set; }
            public bool IsSelected { get; set; }
            public IInventorMethod MethodInstance { get; set; }

            public InventorMethodItem(string name, int priority, Action<global::Inventor.Application> executeAction, IInventorMethod methodInstance, bool isSelected = false)
            {
                Name = name;
                Priority = priority;
                ExecuteAction = executeAction;
                MethodInstance = methodInstance;
                IsSelected = isSelected;
            }
        }

        private List<InventorMethodItem> _methodQueue;
        private global::Inventor.Application _inventorApplication;

        public QueuedInventorMethodsHelper(global::Inventor.Application inventorApplication)
        {
            _inventorApplication = inventorApplication;
            _methodQueue = new List<InventorMethodItem>();
            InitializeMethodQueue();
        }

        /// <summary>
        /// Initialize the method queue with all available Inventor methods
        /// ALL PRIORITIES ARE DEFINED HERE - SINGLE SOURCE OF TRUTH
        /// </summary>
        private void InitializeMethodQueue()
        {
            // Create instances of all Inventor methods
            var retrieveDims = new DrawingRetrieveDims();
            var autoArrange = new AutoArrange();
            var createHoleTable = new CreateHoleTable();
            var addCentermarks = new AutoAddCenterlinesAndCentermarks();

            // CENTRALIZED PRIORITY CONFIGURATION
            // Lower number = higher priority (executes first)

            // Set priorities for each method
            retrieveDims.Priority = 0;    // HIGHEST PRIORITY - Execute first
            autoArrange.Priority = 1;     // SECOND PRIORITY - Execute after retrieve dims
            createHoleTable.Priority = 2; // THIRD PRIORITY - Execute after arrange
            addCentermarks.Priority = 3;  // LOWEST PRIORITY - Future implementation

            // Add methods to queue with their configured priorities
            _methodQueue.Add(new InventorMethodItem(
                retrieveDims.DisplayName,
                retrieveDims.Priority.Value,
                null, // No longer need ExecuteAction delegate since we use the Execute method directly
                retrieveDims
            ));

            _methodQueue.Add(new InventorMethodItem(
                autoArrange.DisplayName,
                autoArrange.Priority.Value,
                null, // No longer need ExecuteAction delegate since we use the Execute method directly
                autoArrange
            ));

            _methodQueue.Add(new InventorMethodItem(
                createHoleTable.DisplayName,
                createHoleTable.Priority.Value,
                null, // No longer need ExecuteAction delegate since we use the Execute method directly
                createHoleTable
            ));

            _methodQueue.Add(new InventorMethodItem(
                addCentermarks.DisplayName,
                addCentermarks.Priority.Value,
                null, // No longer need ExecuteAction delegate since we use the Execute method directly
                addCentermarks
            ));

            // Sort by priority (lowest number = highest priority)
            _methodQueue = _methodQueue.OrderBy(m => m.Priority).ToList();
        }

        /// <summary>
        /// Update method priorities at runtime (allows for dynamic reconfiguration)
        /// </summary>
        public void UpdateMethodPriorities(Dictionary<string, int> newPriorities)
        {
            foreach (var kvp in newPriorities)
            {
                var method = _methodQueue.FirstOrDefault(m => m.Name == kvp.Key);
                if (method != null)
                {
                    method.Priority = kvp.Value;
                    if (method.MethodInstance != null)
                    {
                        method.MethodInstance.Priority = kvp.Value;
                    }
                }
            }

            // Re-sort the queue after priority changes
            _methodQueue = _methodQueue.OrderBy(m => m.Priority).ToList();
        }

        /// <summary>
        /// Get current priority configuration (useful for debugging/logging)
        /// </summary>
        public Dictionary<string, int> GetCurrentPriorities()
        {
            return _methodQueue.ToDictionary(m => m.Name, m => m.Priority);
        }

        /// <summary>
        /// Set which methods are selected for execution
        /// </summary>
        public void SetMethodSelections(bool retrieveDims, bool autoArrange, bool holeTable, bool centermarks)
        {
            var retrieveDimsMethod = _methodQueue.FirstOrDefault(m => m.Name == "Retrieve Dimensions");
            if (retrieveDimsMethod != null) retrieveDimsMethod.IsSelected = retrieveDims;

            var autoArrangeMethod = _methodQueue.FirstOrDefault(m => m.Name == "Auto Arrange");
            if (autoArrangeMethod != null) autoArrangeMethod.IsSelected = autoArrange;

            var holeTableMethod = _methodQueue.FirstOrDefault(m => m.Name == "Add Hole Table");
            if (holeTableMethod != null) holeTableMethod.IsSelected = holeTable;

            var centermarksMethod = _methodQueue.FirstOrDefault(m => m.Name == "Auto Add Centerlines and Centermarks");
            if (centermarksMethod != null) centermarksMethod.IsSelected = centermarks;
        }

        /// <summary>
        /// Execute all selected methods in priority order with a single drawing view selection
        /// </summary>
        /// <returns>List of execution results</returns>
        public List<MethodExecutionResult> ExecuteSelectedMethods()
        {
            var results = new List<MethodExecutionResult>();
            var selectedMethods = _methodQueue.Where(m => m.IsSelected).OrderBy(m => m.Priority);

            if (!selectedMethods.Any())
            {
                results.Add(new MethodExecutionResult
                {
                    MethodName = "Validation",
                    Success = false,
                    Message = "Please select at least one option before clicking Apply.",
                    IsWarning = true
                });
                return results;
            }

            // First, check if we're in a drawing document
            if (_inventorApplication.ActiveDocument.DocumentType != global::Inventor.DocumentTypeEnum.kDrawingDocumentObject)
            {
                results.Add(new MethodExecutionResult
                {
                    MethodName = "Validation",
                    Success = false,
                    Message = "Please open a drawing document before executing operations.",
                    IsCritical = true
                });
                return results;
            }

            // Select the drawing view once for all operations
            global::Inventor.DrawingView selectedView = null;
            try
            {
                selectedView = _inventorApplication.CommandManager.Pick(
                    global::Inventor.SelectionFilterEnum.kDrawingViewFilter,
                    "Select a drawing view for all operations") as global::Inventor.DrawingView;

                if (selectedView == null)
                {
                    results.Add(new MethodExecutionResult
                    {
                        MethodName = "View Selection",
                        Success = false,
                        Message = "No drawing view was selected or selection was cancelled.",
                        IsCritical = true
                    });
                    return results;
                }

                System.Diagnostics.Debug.WriteLine($"Selected drawing view '{selectedView.Name}' for all operations");
            }
            catch (Exception ex)
            {
                results.Add(new MethodExecutionResult
                {
                    MethodName = "View Selection",
                    Success = false,
                    Message = $"Error selecting drawing view: {ex.Message}",
                    Exception = ex,
                    IsCritical = true
                });
                return results;
            }

            // Log execution order for debugging
            System.Diagnostics.Debug.WriteLine("Executing methods in priority order:");
            foreach (var method in selectedMethods)
            {
                System.Diagnostics.Debug.WriteLine($"  Priority {method.Priority}: {method.Name}");
            }

            // Execute all selected methods with the same drawing view
            foreach (var method in selectedMethods)
            {
                var result = ExecuteMethod(method, selectedView);
                results.Add(result);

                // If a critical error occurs, stop execution
                if (!result.Success && result.IsCritical)
                {
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// Execute a single method with the provided drawing view and return the result
        /// </summary>
        private MethodExecutionResult ExecuteMethod(InventorMethodItem method, global::Inventor.DrawingView drawingView)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Executing: {method.Name} (Priority: {method.Priority}) with view: {drawingView.Name}");
                
                // Use the Execute method directly if the method instance is available
                if (method.MethodInstance != null)
                {
                    method.MethodInstance.Execute(_inventorApplication, drawingView);
                }
                else
                {
                    // For methods not yet implemented (like Centermarks), still throw NotImplementedException
                    throw new NotImplementedException($"{method.Name} functionality not yet implemented.");
                }
                
                return new MethodExecutionResult
                {
                    MethodName = method.Name,
                    Success = true,
                    Message = $"{method.Name} executed successfully on view '{drawingView.Name}'!"
                };
            }
            catch (NotImplementedException ex)
            {
                return new MethodExecutionResult
                {
                    MethodName = method.Name,
                    Success = false,
                    Message = ex.Message,
                    IsWarning = true
                };
            }
            catch (Exception ex)
            {
                return new MethodExecutionResult
                {
                    MethodName = method.Name,
                    Success = false,
                    Message = $"Error in {method.Name}: {ex.Message}",
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// Get the priority queue for inspection
        /// </summary>
        public List<InventorMethodItem> GetMethodQueue()
        {
            return _methodQueue.ToList();
        }
    }

    /// <summary>
    /// Result of executing an Inventor method
    /// </summary>
    public class MethodExecutionResult
    {
        public string MethodName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public bool IsWarning { get; set; } = false;
        public bool IsCritical { get; set; } = false;
    }
}
