using System.Collections.Generic;

using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class AppendGroup1 : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection);
        SelectUtils.GroupCache1.AddRange(elements);
        Log.Info($"Group1 count:{SelectUtils.GroupCache1.Count}.");
    }
}
