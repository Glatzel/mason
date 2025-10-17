using System;
using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to unjoin elements between two cached groups.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class GroupUnjoin : AbsCommand
    {
        /// <summary>
        /// Shared pipeline instance for clash and join operations.
        /// </summary>
        internal static readonly ClashAndJoinPipeline pipeline = new();

        /// <summary>
        /// Logger for the command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes the unjoin operation between two element groups.
        /// </summary>
        public override void CommandBody()
        {
            var group1 = SelectUtils.GroupCache1 ?? [];
            var group2 = SelectUtils.GroupCache2 ?? [];

            Log.Info($"Group1 count: {group1.Count}.");
            Log.Info($"Group2 count: {group2.Count}.");

            foreach (ProxyElement e1 in group1)
            {
                foreach (ProxyElement e2 in group2)
                {
                    pipeline
                        .Init(Doc, e1, e2)
                        .IsIdenticalElement(false) // Skip unjoining the same element
                        .IsJoined(true) // Only attempt unjoin if joined
                        .Unjoin();
                }
            }

            Log.Info("Group unjoin operation completed.");
        }
    }
}
