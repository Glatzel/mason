using Autodesk.Revit.DB;
using Mason.Core.Utils;

namespace Mason.Command.Misc;

/// <summary>
/// Resets all graphics overrides for elements in the active view.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ResetGraphicsOverride : Core.AbsCommand(true)
{

    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Executes the command to reset all element graphic overrides in the active view.
    /// </summary>
    public override void CommandBody()
    {
        // Collect all element IDs in the active view
        ElementId[] ids = [.. new FilteredElementCollector(Doc, ActiveView.Id).ToElementIds()];

        if (ids.Length == 0)
        {
            Log.Warn("No elements found in the active view to reset graphics overrides.");

            return;
        }

        // Reset graphics overrides using utility
        Graphics.ResetGraphicsOverride(ids);
        Log.Info(
            $"Reset graphics overrides for {ids.Length} elements in active view `{ActiveView.Name}`."
        );
    }
}
