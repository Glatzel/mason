using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ClearGroup1 : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        SelectUtils.GroupCache1.Clear();
        Log.Debug($"Group1 count:{SelectUtils.GroupCache1.Count}.");
    }
}
