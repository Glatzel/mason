using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class SelectGroup1() : AbsCommand(true)
    {
        public override void CommandBody()
        {
            SelectUtils.GroupCache1 = SelectUtils.SelectProxyElements(Selection);
        }
    }
}
