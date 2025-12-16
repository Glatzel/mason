using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using Mason.Core;

namespace Mason.Command.Misc;

/// <summary>
/// Deletes all CAD imports (ImportInstance) in the active document that are not linked.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ClearCAD() : AbsCommand(true)
{
    /// <summary>Logger for this command.</summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// Deletes all non-linked CAD imports in the active document.
    /// </summary>
    public override void CommandBody()
    {
        // Collect all non-linked CAD import instances
        List<ElementId> cadFiles =
        [
            .. new FilteredElementCollector(Doc)
                .OfClass(typeof(ImportInstance))
                .Cast<ImportInstance>()
                .Where(i => !i.IsLinked)
                .Select(i => i.Id),
        ];

        Log.Info($"Found {cadFiles.Count} CAD import(s) to delete.");

        if (cadFiles.Count == 0)
        {
            Log.Warn("No CAD imports found to delete.");
            return;
        }

        // Delete CAD files
        Doc.Delete(cadFiles);
        Log.Info("Deleted selected CAD import(s) successfully.");
    }
}
