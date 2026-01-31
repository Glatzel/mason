using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Mason.Core.Utils;

namespace Mason.Command.Misc;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ExportWorksetsNWC : Core.AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        //select path
        string outPath = IO.SelectFolder();

        // get all worksets
        IList<Workset> worksets = new FilteredWorksetCollector(Doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();
        if (worksets.Count <= 1)
        {
            Log.Warn("Only one Workset found.");
            TaskDialog.Show(
                GetType().FullName,
                "Only one Workset found.\nPlease export NWC Manually."
            );
            return;
        }
        Log.Info($"Find {worksets.Count} Worksets in {Doc.Title}.rvt");

        // get current view for export
        View view = Doc.ActiveView;
        Log.Info($"Active View: {view.Name}");

        // export setting
        NavisworksExportOptions option = new()
        {
            ExportScope = NavisworksExportScope.View,
            ViewId = view.Id,
            ExportRoomGeometry = false,
        };

        // export nwc
        foreach (Workset w in worksets)
        {
            Log.Info($"Start Export NWC of Workset: {w.Name}");
            //Isolate elements
            using (Transaction tsIsolate = new(Doc, "test"))
            {
                tsIsolate.Start();
                foreach (Workset i in worksets)
                {
                    view.SetWorksetVisibility(i.Id, WorksetVisibility.Hidden);
                }
                view.SetWorksetVisibility(w.Id, WorksetVisibility.Visible);
                tsIsolate.Commit();
            }
            // check if nothing in view
            IEnumerable<Element> modelsInView = new FilteredElementCollector(Doc, ActiveView.Id)
                .ToElements()
                .Where(e => e.Category?.CategoryType == CategoryType.Model);
            if (!modelsInView.Any())
            {
                Log.Warn($"No model of workset `{w.Name}` in view `{ActiveView.Name}`");
                continue;
            }
            // export
            string outFile = Path.Combine(outPath, $"{w.Name}.nwc");
            if (!File.Exists(outFile))
            {
                Log.Info($"NWC file already exists: {outFile}");
            }
            Doc.Export(outPath, $"{w.Name.Replace('/', '_')}", option);
            Log.Info($"Finish Export NWC of Workset: {w.Name}");
        }

        // reset view visibility
        using Transaction ts = new(Doc, "test");
        ts.Start();
        worksets.ToList().ForEach(w => view.SetWorksetVisibility(w.Id, WorksetVisibility.Visible));
        ts.Commit();
    }
}
