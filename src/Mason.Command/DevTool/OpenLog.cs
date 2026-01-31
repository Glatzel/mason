using NLog;

namespace Mason.Command.DevTool;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class OpenLog : Core.AbsCommand
{
    private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        NLog.Targets.FileTarget target =
            LogManager.Configuration?.FindTargetByName<NLog.Targets.FileTarget>("MasonLogFile");
        string logFile = target.FileName.ToString();
        Log.Debug($"Open Log File: {logFile}");
        System.Diagnostics.Process.Start(logFile);
    }
}
