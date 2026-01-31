using System.Collections.Generic;

using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class AppendGroup2 : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection);
        SelectUtils.GroupCache2.AddRange(elements);
        Log.Info($"Group2 count:{SelectUtils.GroupCache2.Count}.");
    }
}
