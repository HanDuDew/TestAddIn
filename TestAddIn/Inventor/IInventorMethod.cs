using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBeau.Inventor
{
    /// <summary>
    /// Base interface for all Inventor method classes
    /// Provides a consistent Priority property that can be set externally
    /// </summary>
    public interface IInventorMethod
    {
        /// <summary>
        /// Priority for execution order (lower number = higher priority)
        /// Default value is null, should be set by QueuedInventorMethodsHelper
        /// </summary>
        int? Priority { get; set; }
        
        /// <summary>
        /// Display name for the method (used in UI and logging)
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Execute the Inventor method with the provided Inventor Application and Drawing View
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The selected drawing view to operate on</param>
        void Execute(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView);
    }

    /// <summary>
    /// Abstract base class for Inventor methods providing default Priority implementation
    /// </summary>
    public abstract class InventorMethodBase : IInventorMethod
    {
        /// <summary>
        /// Priority for execution order (lower number = higher priority)
        /// Default value is null, should be set by QueuedInventorMethodsHelper
        /// </summary>
        public int? Priority { get; set; } = null;

        /// <summary>
        /// Display name for the method (used in UI and logging)
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Abstract method that must be implemented by derived classes to define their execution logic
        /// </summary>
        /// <param name="inventorApp">The Inventor Application object</param>
        /// <param name="drawingView">The selected drawing view to operate on</param>
        public abstract void Execute(global::Inventor.Application inventorApp, global::Inventor.DrawingView drawingView);
    }
}