using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelectGroup2 : AbsCommand
{
    public override void CommandBody()
    {
        SelectUtils.GroupCache2 = SelectUtils.SelectProxyElements(Selection);
    }
}
