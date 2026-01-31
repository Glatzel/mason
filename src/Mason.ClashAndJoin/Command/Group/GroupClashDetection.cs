using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class GroupClashDetection : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    internal static ClashAndJoinPipeline pipeline = new();

    public override void CommandBody()
    {
        Log.Info($"Group1 count:{SelectUtils.GroupCache1.Count}.");
        Log.Info($"Group2 count:{SelectUtils.GroupCache2.Count}.");
        foreach (ProxyElement e1 in SelectUtils.GroupCache1)
        {
            try
            {
                e1.Filter = new Autodesk.Revit.DB.ElementIntersectsElementFilter(e1.E);
                foreach (ProxyElement e2 in SelectUtils.GroupCache2)
                {
                    bool isIntersect = pipeline
                        .Init(Doc, e1, e2)
                        .IsIdenticalElement(false)
                        .IsBoundingBoxIntersect(true)
                        .IsJoined(false)
                        .ClashDetection();
                }
            }
            catch { }
        }
    }
}
