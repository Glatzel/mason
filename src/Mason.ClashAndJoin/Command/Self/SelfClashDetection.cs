using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Self;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelfClashDetection : AbsCommand
{
    internal static ClashAndJoinPipeline pipeline = new();

    public override void CommandBody()
    {
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection);
        elements.ForEach(e => e.InitFilter());
        for (int i = 0; i < elements.Count - 1; i++)
        {
            ProxyElement e1 = elements[i];
            for (int j = i + 1; j < elements.Count; j++)
            {
                ProxyElement e2 = elements[j];
                bool isIntersect = pipeline
                    .Init(Doc, e1, e2)
                    .IsBoundingBoxIntersect(true)
                    .IsJoined(false)
                    .ClashDetection();
            }
        }
    }
}
