using System;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to clear the cached second group of selected elements.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ClearGroup2 : AbsCommand
    {
        /// <summary>
        /// Logger for this command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the command to clear the second group cache.
        /// </summary>
        public override void CommandBody()
        {
            try
            {
                if (SelectUtils.GroupCache2 == null)
                {
                    Log.Warn("GroupCache2 is null. Initializing new list.");
                    SelectUtils.GroupCache2 = [];
                }

                SelectUtils.GroupCache2.Clear();
                Log.Info($"Group2 cleared. Current count: {SelectUtils.GroupCache2.Count}.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to clear GroupCache2.");
            }
        }
    }
}
