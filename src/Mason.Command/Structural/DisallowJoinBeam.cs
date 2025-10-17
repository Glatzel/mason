using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace Mason.Command.Structural;

/// <summary>
/// Disallows joining at both ends of selected structural framing elements (beams).
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class DisallowJoinBeam : Core.AbsCommand
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

        // Filter for structural framing elements (beams)
        IEnumerable<FamilyInstance> beams =
        [
            .. new FilteredElementCollector(Doc, selectedElements)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsNotElementType()
                .OfType<FamilyInstance>(),
        ];

        Log.Info($"Selected {beams.Count()} beam(s) to disallow joins.");

        // Disallow joining at both ends for each beam
        foreach (FamilyInstance beam in beams)
        {
            StructuralFramingUtils.DisallowJoinAtEnd(beam, 0);
            StructuralFramingUtils.DisallowJoinAtEnd(beam, 1);
        }

        Log.Info("Finished disallowing joins on selected beams.");
    }
}
