using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Bench;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class BenchClashDetect : AbsCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<Element> elements =
        [
            .. Selection.PickObjects(ObjectType.Element).Select(Doc.GetElement),
        ];
        long timeScalar = MethodScalar(elements);
        long timeVector = MethodVector(elements);
        TaskDialog.Show(
            "Clash Benchmark",
            $"Scalar time: {timeScalar / 1000.0}s\nVector time: {timeVector / 1000.0}s\n"
        );
    }

    public static long MethodScalar(List<Element> elements)
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        ElementIntersectsElementFilter filter = new(elements[0]);
        for (int i = 1; i < elements.Count; i++)
        {
            filter.PassesFilter(elements[i]);
        }
        watch.Stop();
        Log.Info($"Scalar clash time: {watch.ElapsedMilliseconds}ms");
        return watch.ElapsedMilliseconds;
    }

    public static long MethodVector(List<Element> elements)
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        ElementIntersectsElementFilter filter = new(elements[0]);
        new FilteredElementCollector(
            Doc,
            elements.GetRange(1, elements.Count - 1).ConvertAll(e => e.Id)
        ).WherePasses(filter);
        watch.Stop();
        Log.Info($"Scalar clash time: {watch.ElapsedMilliseconds}ms");
        return watch.ElapsedMilliseconds;
    }
}
