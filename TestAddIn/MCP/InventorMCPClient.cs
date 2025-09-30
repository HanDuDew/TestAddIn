using System;
using System.Collections.Generic;

namespace AutoBeau.MCP
{
    /// <summary>
    /// Simple facade client that would (in a full MCP implementation) serialize requests.
    /// For now it just calls the singleton server directly.
    /// </summary>
    internal class InventorMCPClient
    {
        public IList<object> ListMethods()
        {
            return InventorMCPServer.Instance.ListMethods();
        }

        public object SetSelections(bool retrieveDims, bool autoArrange, bool holeTable, bool centermarks)
        {
            return InventorMCPServer.Instance.SetSelections(retrieveDims, autoArrange, holeTable, centermarks);
        }

        public object ExecuteMethod(string methodName, string drawingViewName = null)
        {
            return InventorMCPServer.Instance.ExecuteMethod(methodName, drawingViewName);
        }

        public object ExecuteSelectedQueue()
        {
            return InventorMCPServer.Instance.ExecuteSelectedQueue();
        }
    }
}