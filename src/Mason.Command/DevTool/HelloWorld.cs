using Autodesk.Revit.UI;

namespace Mason.Command.DevTool;

/// <summary>
/// A simple Revit command that demonstrates basic usage by showing a "Hello World" message.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class HelloWorld : Core.AbsCommand
{
    /// <summary>Logger for the command.</summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// The main body of the command executed in Revit.
    /// </summary>
    public override void CommandBody()
    {
        // Log the start of the command execution
        Log.Info("Executing HelloWorld command.");

        // Show a simple message dialog in Revit
        TaskDialog.Show("Revit", "Hello World");

        // Log the successful completion of the command
        Log.Info("HelloWorld command completed successfully.");
    }
}
