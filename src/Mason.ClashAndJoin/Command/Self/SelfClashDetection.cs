using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self
{
    /// <summary>
    /// Command to perform clash detection between selected elements in Revit.
    /// Checks all unique element pairs for bounding box intersection and join status,
    /// then runs the clash detection filter for each pair.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class SelfClashDetection : AbsCommand
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
        /// Executes clash detection for all selected elements.
        /// </summary>
        public override void CommandBody()
        {
            // Select elements from the current selection
            List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection) ?? [];

            if (elements.Count == 0)
            {
                Log.Warn("No elements selected for clash detection.");
                return;
            }

            // Initialize filters for all elements
            elements.ForEach(e =>
            {
                try
                {
                    e.InitFilter();
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex, $"Failed to initialize filter for element {e.Id}.");
                }
            });

            // Iterate through all unique pairs of elements
            for (int i = 0; i < elements.Count - 1; i++)
            {
                ProxyElement e1 = elements[i];

                for (int j = i + 1; j < elements.Count; j++)
                {
                    ProxyElement e2 = elements[j];

                    try
                    {
                        // Run pipeline: check bounding box intersection, not joined, then clash detection
                        bool isIntersect = pipeline
                            .Init(Doc, e1, e2)
                            .IsBoundingBoxIntersect(true)
                            .IsJoined(false)
                            .ClashDetection();

                        // Optionally log or handle clash result
                        if (isIntersect)
                        {
                            Log.Info($"Clash detected between elements {e1.Id} and {e2.Id}.");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(
                            ex,
                            $"Failed clash detection between elements {e1.Id} and {e2.Id}."
                        );
                    }
                }
            }

            Log.Info($"Clash detection completed for {elements.Count} selected elements.");
        }
    }
}
