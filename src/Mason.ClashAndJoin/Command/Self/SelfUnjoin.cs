using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self;

/// <summary>
/// Command to unjoin all selected elements in the Revit document.
/// Iterates over all selected ProxyElements and unjoins any joined pairs.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelfUnjoin() : AbsCommand(true)
{
    /// <summary>
    /// Pipeline for clash and join operations.
    /// </summary>
    internal static readonly ClashAndJoinPipeline pipeline = new();

    /// <summary>
    /// Logger for command operations.
    /// </summary>
    internal static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Executes the unjoin operation on all selected ProxyElements.
    /// </summary>
    public override void CommandBody()
    {
        // Select elements from the current selection
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection) ?? [];

        if (elements.Count == 0)
        {
            Log.Warn("No elements selected for unjoining.");
            return;
        }

        // Iterate over all unique pairs of elements
        for (int i = 0; i < elements.Count; i++)
        {
            ProxyElement e1 = elements[i];

            for (int j = i + 1; j < elements.Count; j++)
            {
                ProxyElement e2 = elements[j];

                // Initialize pipeline for this pair, check if joined, and unjoin
                pipeline.Init(Doc, e1, e2).IsJoined(true).Unjoin();
            }
        }

        Log.Info($"Unjoin operation completed for {elements.Count} selected elements.");
    }
}
