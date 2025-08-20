using Autodesk.Revit.UI;

namespace Mason.Command.DevTool;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class HelloWorld : Core.AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        Log.Info("Hello World");
        TaskDialog.Show("Revit", "Hello World");
    }
}
