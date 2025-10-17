using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self;

/// <summary>
/// Command to automatically join selected elements in Revit.
/// Iterates through all unique element pairs and joins them
/// if their bounding boxes intersect and they are not already joined.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelfJoin : AbsCommand
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
    /// Initializes a new instance of <see cref="SelfJoin"/>.
    /// </summary>
    public SelfJoin()
        : base(true) { }

    /// <summary>
    /// Executes the join operation for all selected elements.
    /// </summary>
    public override void CommandBody()
    {
        // Select elements from the current selection
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection) ?? [];

        if (elements.Count == 0)
        {
            Log.Warn("No elements selected for joining.");
            return;
        }

        // Iterate through all unique pairs of elements
        for (int i = 0; i < elements.Count - 1; i++)
        {
            ProxyElement e1 = elements[i];

            for (int j = i + 1; j < elements.Count; j++)
            {
                ProxyElement e2 = elements[j];

                // Initialize pipeline for this pair, check intersection, not joined, then join
                pipeline.Init(Doc, e1, e2).IsBoundingBoxIntersect(true).IsJoined(false).Join();
            }
        }

        Log.Info($"Join operation completed for {elements.Count} selected elements.");
    }
}
