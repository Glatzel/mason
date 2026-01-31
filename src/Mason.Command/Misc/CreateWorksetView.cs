using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Mason.Command.Misc;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class CreateWorksetView : Core.AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        // get all worksets
        IList<Workset> worksets = new FilteredWorksetCollector(Doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();
        if (worksets.Count <= 1)
        {
            Log.Warn("Only one Workset found.");
            TaskDialog.Show(
                GetType().FullName,
                "Only one Workset found.\nNo View will be created."
            );
            return;
        }
        Log.Info($"Find {worksets.Count} Worksets in {Doc.Title}.rvt");
        //get element of 3D view
        ViewFamilyType vft = new FilteredElementCollector(Doc)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .FirstOrDefault(x => ViewFamily.ThreeDimensional == x.ViewFamily);
        // create view need transaction
        Log.Info("Start Create View for worksets.");
        using Transaction ts = new(Doc, "test");
        ts.Start();
        for (int i = 0; i < worksets.Count; i++)
        {
            try
            {
                View3D view = View3D.CreateIsometric(Doc, vft.Id);
                view.Name = $"{i}.{worksets[i].Name}";
                // hide all worksets
                foreach (Workset w in worksets)
                {
                    view.SetWorksetVisibility(w.Id, WorksetVisibility.Hidden);
                }
                // display one workset.
                view.SetWorksetVisibility(worksets[i].Id, WorksetVisibility.Visible);
                Log.Info($"Create View: {i}.{worksets[i].Name}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        ts.Commit();
    }
}
