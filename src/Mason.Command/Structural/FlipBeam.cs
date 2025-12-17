using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Mason.Command.Structural;

/// <summary>
/// Flips the ends of selected structural framing elements (beams) that can be flipped.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class FlipBeam() : Core.AbsCommand(true)
{
    /// <summary>Logger for the command.</summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Prompt user to select elements
        List<ElementId> selectedElements =
        [
            .. Selection.PickObjects(ObjectType.Element).Select(r => r.ElementId),
        ];

        if (selectedElements.Count == 0)
        {
            TaskDialog.Show("Flip Beam", "No elements selected.");
            return;
        }

        // Filter for structural framing elements (beams) that can be flipped
        IEnumerable<FamilyInstance> beams =
        [
            .. new FilteredElementCollector(Doc, selectedElements)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .OfType<FamilyInstance>()
                .Where(StructuralFramingUtils.CanFlipEnds),
        ]; // Evaluate once

        Log.Info($"Selected {beams.Count()} beam(s) for flipping.");

        // Flip ends of each beam
        foreach (FamilyInstance beam in beams)
        {
            StructuralFramingUtils.FlipEnds(beam);
        }

        Log.Info("Finished flipping selected beams.");
    }
}
