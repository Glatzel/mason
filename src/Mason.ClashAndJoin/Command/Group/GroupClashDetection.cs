using System;
using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    /// <summary>
    /// Command to perform clash detection between two cached groups of elements.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class GroupClashDetection : AbsCommand
    {
        /// <summary>
        /// Logger for this command.
        /// </summary>
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Shared pipeline instance for clash and join operations.
        /// </summary>
        internal static readonly ClashAndJoinPipeline pipeline = new();

        /// <summary>
        /// Executes the clash detection between elements in two groups.
        /// </summary>
        public override void CommandBody()
        {
            try
            {
                // Ensure the group caches are not null
                var group1 = SelectUtils.GroupCache1 ?? [];
                var group2 = SelectUtils.GroupCache2 ?? [];

                Log.Info($"Group1 count: {group1.Count}.");
                Log.Info($"Group2 count: {group2.Count}.");

                foreach (ProxyElement e1 in group1)
                {
                    try
                    {
                        // Initialize a Revit filter for the element
                        e1.Filter = new Autodesk.Revit.DB.ElementIntersectsElementFilter(e1.E);

                        foreach (ProxyElement e2 in group2)
                        {
                            try
                            {
                                // Perform clash detection using the pipeline
                                bool isIntersect = pipeline
                                    .Init(Doc, e1, e2)
                                    .IsIdenticalElement(false) // Skip identical elements
                                    .IsBoundingBoxIntersect(true) // Only consider intersecting bounding boxes
                                    .IsJoined(false) // Only consider elements not already joined
                                    .ClashDetection();

                                if (isIntersect)
                                {
                                    Log.Debug($"Clash detected: {e1.Id} ↔ {e2.Id}");
                                }
                            }
                            catch (Exception exInner)
                            {
                                Log.Warn(
                                    exInner,
                                    $"Failed to process element pair: {e1.Id}, {e2.Id}"
                                );
                            }
                        }
                    }
                    catch (Exception exElement)
                    {
                        Log.Warn(exElement, $"Failed to process element: {e1.Id}");
                    }
                }

                Log.Info("Group clash detection completed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to execute GroupClashDetection command.");
            }
        }
    }
}
