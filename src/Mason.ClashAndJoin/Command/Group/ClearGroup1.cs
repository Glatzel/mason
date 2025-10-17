using System;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to clear the cached first group of selected elements.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ClearGroup1 : AbsCommand
    {
        /// <summary>
        /// Logger for this command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the command to clear the first group cache.
        /// </summary>
        public override void CommandBody()
        {
            try
            {
                if (SelectUtils.GroupCache1 == null)
                {
                    Log.Warn("GroupCache1 is null. Initializing new list.");
                    SelectUtils.GroupCache1 = [];
                }

                SelectUtils.GroupCache1.Clear();
                Log.Info($"Group1 cleared. Current count: {SelectUtils.GroupCache1.Count}.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to clear GroupCache1.");
            }
        }
    }
}
