using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ClearAll : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        SelectUtils.GroupCache1.Clear();
        SelectUtils.GroupCache2.Clear();
        Log.Debug($"Group1 count:{SelectUtils.GroupCache1}.");
        Log.Debug($"Group2 count:{SelectUtils.GroupCache2}.");
    }
}
