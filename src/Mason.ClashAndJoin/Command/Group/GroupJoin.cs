using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class GroupJoin() : AbsCommand(true)
{
    internal static ClashAndJoinPipeline pipeline = new();
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        Log.Info($"Group1 count:{SelectUtils.GroupCache1.Count}.");
        Log.Info($"Group2 count:{SelectUtils.GroupCache2.Count}.");
        foreach (ProxyElement e1 in SelectUtils.GroupCache1)
        {
            foreach (ProxyElement e2 in SelectUtils.GroupCache2)
            {
                pipeline
                    .Init(Doc, e1, e2)
                    .IsIdenticalElement(false)
                    .IsBoundingBoxIntersect(true)
                    .IsJoined(false)
                    .Join();
            }
        }
    }
}
