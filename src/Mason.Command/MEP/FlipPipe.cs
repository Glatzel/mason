using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using Mason.Core;

namespace Mason.Command.MEP;

/// <summary>
/// Flips the direction of selected pipe elements by reversing their location curves.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class FlipPipe() : AbsCommand(true)
{
    /// <summary>Logger for the command.</summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    public override void CommandBody()
    {
        // Prompt user to select elements and filter for pipes
        IEnumerable<Pipe> pipes =
        [
            .. Selection
                .PickObjects(ObjectType.Element)
                .Select(id => Doc.GetElement(id))
                .OfType<Pipe>(),
        ];

        Log.Info($"Selected {pipes.Count()} pipe(s) for flipping.");

        // Reverse the curve of each pipe
        foreach (Pipe pipe in pipes)
        {
            if (pipe.Location is LocationCurve locationCurve)
            {
                locationCurve.Curve = locationCurve.Curve.CreateReversed();
            }
            else
            {
                Log.Warn($"Pipe {pipe.Id} does not have a LocationCurve and was skipped.");
            }
        }

        Log.Info("Finished flipping selected pipes.");
    }
}
