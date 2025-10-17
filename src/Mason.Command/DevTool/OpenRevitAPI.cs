using System;
using NLog;

namespace Mason.Command.DevTool;

/// <summary>
/// Opens the Autodesk Revit API documentation for the current Revit version in the default web browser.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class OpenRevitAPI : Core.AbsCommand
{
    /// <summary>Logger for the command.</summary>
    private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Construct the URL for the Revit API documentation
        string target = $"https://apidocs.co/apps/revit/{UIApp.Application.VersionNumber}/";

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new InvalidOperationException("Revit API documentation URL is invalid.");
        }

        // Launch the URL in the default web browser
        System.Diagnostics.Process.Start(
            new System.Diagnostics.ProcessStartInfo(target) { UseShellExecute = true }
        );

        // Log the URL that was opened
        Log.Debug($"Opened Revit API documentation: {target}");
    }
}
