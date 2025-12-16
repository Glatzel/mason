using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self;

/// <summary>
/// Command to join wall openings automatically.
/// Iterates through selected ProxyElements and joins pairs
/// whose bounding boxes intersect and are not already joined.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelfJoinWallOpening : AbsCommand
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
    /// Initializes a new instance of <see cref="SelfJoinWallOpening"/>.
    /// </summary>
    public SelfJoinWallOpening()
        : base(true) { }

    /// <summary>
    /// Executes the wall opening join operation.
    /// </summary>
    public override void CommandBody()
    {
        // Select elements from the current selection; 'false' indicates some filtering option
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection, false) ?? [];

        if (elements.Count == 0)
        {
            Log.Warn("No elements selected for wall opening joining.");
            return;
        }

        // Precompute bounding boxes with slight offset for safety
        elements.ForEach(e =>
        {
            e.CachedBBox = e.BBox.Offset(1);
        });

        // Iterate over all unique pairs of elements
        for (int i = 0; i < elements.Count - 1; i++)
        {
            ProxyElement e1 = elements[i];

            for (int j = i + 1; j < elements.Count; j++)
            {
                ProxyElement e2 = elements[j];

                // Initialize pipeline for this pair, check intersection, not joined, and join
                pipeline.Init(Doc, e1, e2).IsBoundingBoxIntersect(true).IsJoined(false).Join();
            }
        }

        Log.Info($"Wall opening join operation completed for {elements.Count} selected elements.");
    }
}
