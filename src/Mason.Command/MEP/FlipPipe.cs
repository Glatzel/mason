using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Mason.Command.MEP;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class FlipPipe() : Core.AbsCommand(true)
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        IEnumerable<Pipe> pipes = Selection
            .PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element)
            .Select(Doc.GetElement)
            .OfType<Pipe>();
        Log.Info($"Select {pipes.Count()} Pipes.");
        foreach (Pipe p in pipes)
        {
            LocationCurve LocationCurve = p.Location as LocationCurve;
            LocationCurve.Curve = LocationCurve.Curve.CreateReversed();
        }
    }
}
