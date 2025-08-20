using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Mason.Command.Structural;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class FlipBeam() : Core.AbsCommand(true)
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<ElementId> selectedElements = Selection
            .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element)
            .Select(r => r.ElementId)
            .ToList();

        IEnumerable<FamilyInstance> beams = new FilteredElementCollector(Doc, selectedElements)
            .OfCategory(BuiltInCategory.OST_StructuralFraming)
            .WhereElementIsNotElementType()
            .OfType<FamilyInstance>()
            .Where(StructuralFramingUtils.CanFlipEnds);

        Log.Info($"Select {beams.Count()} Beams.");
        foreach (FamilyInstance beam in beams)
        {
            StructuralFramingUtils.FlipEnds(beam);
        }
    }
}
