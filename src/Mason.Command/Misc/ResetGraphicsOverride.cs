using Autodesk.Revit.DB;

namespace Mason.Command.Misc;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ResetGraphicsOverride() : Core.AbsCommand(true)
{
    public override void CommandBody()
    {
        ElementId[] ids = [.. new FilteredElementCollector(Doc, ActiveView.Id).ToElementIds()];

        Core.Utils.Graphics.ResetGraphicsOverride(ids);
    }
}
