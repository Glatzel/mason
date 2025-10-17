using System;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to clear both cached groups of selected elements.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ClearAll : AbsCommand
    {
        /// <summary>
        /// Logger for this command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the command to clear both group caches.
        /// </summary>
        public override void CommandBody()
        {

                if (SelectUtils.GroupCache1 == null)
                {
                    Log.Warn("GroupCache1 is null. Initializing new list.");
                    SelectUtils.GroupCache1 = [];
                }
                if (SelectUtils.GroupCache2 == null)
                {
                    Log.Warn("GroupCache2 is null. Initializing new list.");
                    SelectUtils.GroupCache2 = [];
                }

                SelectUtils.GroupCache1.Clear();
                SelectUtils.GroupCache2.Clear();

                Log.Info($"Group1 cleared. Current count: {SelectUtils.GroupCache1.Count}.");
                Log.Info($"Group2 cleared. Current count: {SelectUtils.GroupCache2.Count}.");

        }
    }
}
