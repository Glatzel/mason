using System;

using Mason.Core;

using NLog;

namespace Mason.Command.DevTool;

/// <summary>
/// Opens the latest Mason log file in the default system viewer.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class OpenLog() : AbsCommand(false)
{
    /// <summary>Logger for the command.</summary>
    private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Find the configured Mason log file target
        NLog.Targets.FileTarget target =
            LogManager.Configuration?.FindTargetByName<NLog.Targets.FileTarget>("MasonLogFile")
            ?? throw new InvalidOperationException("Mason log target not found.");

        // Resolve the log file path (FileName may be a layout, convert to string)
        string logFile = target.FileName.ToString();
        if (string.IsNullOrEmpty(logFile))
        {
            throw new InvalidOperationException("Log file path is empty.");
        }

        // Log the action of opening the file
        Log.Debug($"Opening Mason log file: {logFile}");

        // Launch the log file in the default system viewer
        System.Diagnostics.Process.Start(
            new System.Diagnostics.ProcessStartInfo(logFile) { UseShellExecute = true }
        );
    }
}
