using System.Collections.Generic;

using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelfJoinWallOpening() : AbsCommand(true)
{
    internal static ClashAndJoinPipeline pipeline = new();

    public override void CommandBody()
    {
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection, false);
        elements.ForEach(e => e.CachedBBox = e.BBox.Offset(1));

        for (int i = 0; i < elements.Count - 1; i++)
        {
            ProxyElement e1 = elements[i];
            for (int j = i + 1; j < elements.Count; j++)
            {
                ProxyElement e2 = elements[j];
                pipeline.Init(Doc, e1, e2).IsBoundingBoxIntersect(true).IsJoined(false).Join();
            }
        }
    }
}
