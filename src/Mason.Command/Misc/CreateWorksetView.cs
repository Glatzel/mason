using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Mason.Core;

namespace Mason.Command.Misc;

/// <summary>
/// Creates individual 3D views for each user workset in the active Revit document.
/// Each view hides all worksets except the one it represents.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class CreateWorksetView() : AbsCommand(false)
{
    /// <summary>Logger for this command.</summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Collect all user worksets
        IList<Workset> worksets = new FilteredWorksetCollector(Doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();

        if (worksets.Count <= 1)
        {
            Log.Warn("Only one user workset found. No views will be created.");
            TaskDialog.Show(
                GetType().FullName,
                "Only one user workset found.\nNo views will be created."
            );
            return;
        }

        Log.Info($"Found {worksets.Count} user worksets in {Doc.Title}.");

        // Get a 3D view family type
        ViewFamilyType vft =
            new FilteredElementCollector(Doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional)
            ?? throw new InvalidOperationException("No 3D ViewFamilyType found.");

        Log.Info("Starting creation of 3D views for worksets.");

        using Transaction ts = new(Doc, "Create Workset Views");
        ts.Start();

        for (int i = 0; i < worksets.Count; i++)
        {
            try
            {
                Workset currentWorkset = worksets[i];
                View3D view =
                    View3D.CreateIsometric(Doc, vft.Id)
                    ?? throw new InvalidOperationException("Failed to create 3D view.");

                view.Name = $"{i}.{currentWorkset.Name}";

                // Hide all worksets in this view
                foreach (Workset w in worksets)
                {
                    view.SetWorksetVisibility(w.Id, WorksetVisibility.Hidden);
                }

                // Make current workset visible
                view.SetWorksetVisibility(currentWorkset.Id, WorksetVisibility.Visible);

                Log.Info($"Created view: {view.Name}");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to create view for workset: {worksets[i].Name}");
            }
        }

        ts.Commit();
        Log.Info("Completed creating 3D views for all worksets.");
    }
}
