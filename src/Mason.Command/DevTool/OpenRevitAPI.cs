namespace Mason.Command.DevTool;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class OpenRevitAPI : Core.AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        string target = $"https://apidocs.co/apps/revit/{UIApp.Application.VersionNumber}/";
        System.Diagnostics.Process.Start(target);
        Log.Debug($"Open Revit API: {target}");
    }
}
