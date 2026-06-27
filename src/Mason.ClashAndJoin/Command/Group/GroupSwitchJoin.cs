using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

/// <summary>
/// Command to switch the join status of elements between two cached groups.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class GroupSwitchJoin() : AbsCommand(true)
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
    /// Executes the switch join operation between two element groups.
    /// </summary>
    public override void CommandBody()
    {
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
                    .IsIdenticalElement(false) // Skip switching join for identical elements
                    .IsJoined(true) // Only attempt if currently joined
                    .SwitchJoin();
            }
        }

        Log.Info("Group switch join operation completed.");
    }
}
