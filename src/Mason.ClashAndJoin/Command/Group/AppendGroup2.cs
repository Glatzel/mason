using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to append selected elements to GroupCache2.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class AppendGroup2 : AbsCommand
    {
        /// <summary>
        /// Logger for this command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the command to append selected elements to GroupCache2.
        /// </summary>
        public override void CommandBody()
        {
            // Select elements from current selection
            List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection);

            if (elements.Count == 0)
            {
                Log.Info("No elements selected to append to Group2.");
                return;
            }

            // Ensure GroupCache2 is initialized
            if (SelectUtils.GroupCache2 == null)
            {
                Log.Warn("GroupCache2 is null. Initializing new list.");
                SelectUtils.GroupCache2 = [];
            }

            // Append selected elements
            SelectUtils.GroupCache2.AddRange(elements);

            Log.Info(
                $"Appended {elements.Count} elements to Group2. Current count: {SelectUtils.GroupCache2.Count}."
            );
        }
    }
}
