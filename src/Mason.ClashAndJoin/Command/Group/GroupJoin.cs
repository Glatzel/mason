using Mason.Core;
using System.Collections.Generic;

namespace Mason.ClashAndJoin.Command.Group;

/// <summary>
/// Command to join elements between two cached groups if they are not already joined and their bounding boxes intersect.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class GroupJoin : AbsCommand
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
    /// Executes the join operation between two element groups.
    /// </summary>
    public override void CommandBody()
    {
        // Ensure the group caches are not null
        List<ProxyElement> group1 = SelectUtils.GroupCache1 ?? [];
        List<ProxyElement> group2 = SelectUtils.GroupCache2 ?? [];

        Log.Info($"Group1 count: {group1.Count}.");
        Log.Info($"Group2 count: {group2.Count}.");

        foreach (ProxyElement e1 in group1)
        {
            foreach (ProxyElement e2 in group2)
            {
                pipeline
                    .Init(Doc, e1, e2)
                    .IsIdenticalElement(false) // Skip identical elements
                    .IsBoundingBoxIntersect(true) // Only join if bounding boxes intersect
                    .IsJoined(false) // Only join if not already joined
                    .Join();
            }
        }

        Log.Info("Group join operation completed.");
    }
}
