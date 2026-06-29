using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Mason.Core;
using Mason.Core.Utils;

namespace Mason.Command.Misc;

/// <summary>
/// Exports each user workset in the active Revit document to individual NWC files.
/// Each export isolates the workset in the active view.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ExportWorksetsNWC() : AbsCommand(false)
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Prompt user to select output folder
        string outPath = IO.SelectFolder();
        if (string.IsNullOrWhiteSpace(outPath))
        {
            throw new InvalidOperationException("Output path not selected.");
        }

        // Collect all user worksets
        IList<Workset> worksets = new FilteredWorksetCollector(Doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();

        if (worksets.Count <= 1)
        {
            Log.Warn("Only one user workset found. Manual export is recommended.");
            TaskDialog.Show(
                GetType().FullName,
                "Only one user workset found.\nPlease export NWC manually."
            );
            return;
        }

        Log.Info($"Found {worksets.Count} user worksets in {Doc.Title}.");

        // Get active view for export
        View activeView =
            Doc.ActiveView ?? throw new InvalidOperationException("No active view found.");
        Log.Info($"Active View: {activeView.Name}");

        // Prepare Navisworks export options
        NavisworksExportOptions options = new()
        {
            ExportScope = NavisworksExportScope.View,
            ViewId = activeView.Id,
            ExportRoomGeometry = false,
        };

        // Export each workset individually
        foreach (Workset workset in worksets)
        {
            Log.Info($"Starting NWC export for Workset: {workset.Name}");

            // Isolate the current workset
            using (Transaction tsIsolate = new(Doc, $"Isolate Workset: {workset.Name}"))
            {
                tsIsolate.Start();
                foreach (Workset w in worksets)
                {
                    activeView.SetWorksetVisibility(w.Id, WorksetVisibility.Hidden);
                }
                activeView.SetWorksetVisibility(workset.Id, WorksetVisibility.Visible);
                tsIsolate.Commit();
            }

            // Check if the workset has any model elements in the view
            IEnumerable<Element> modelsInView = new FilteredElementCollector(Doc, activeView.Id)
                .ToElements()
                .Where(e => e.Category?.CategoryType == CategoryType.Model);

            if (!modelsInView.Any())
            {
                Log.Warn(
                    $"No model elements found for workset `{workset.Name}` in view `{activeView.Name}`."
                );
                continue;
            }

            // Build output file path
            string safeName = workset.Name.Replace('/', '_');
            string outFile = Path.Combine(outPath, $"{safeName}.nwc");

            if (File.Exists(outFile))
            {
                Log.Info($"NWC file already exists: {outFile}");
            }

            // Perform export
            Doc.Export(outPath, safeName, options);
            Log.Info($"Finished NWC export for Workset: {workset.Name}");
        }

        // Reset all worksets to visible
        using Transaction tsReset = new(Doc, "Reset Workset Visibility");
        tsReset.Start();
        foreach (Workset w in worksets)
        {
            activeView.SetWorksetVisibility(w.Id, WorksetVisibility.Visible);
        }
        tsReset.Commit();

        Log.Info("Completed NWC export for all worksets.");
    }
}
