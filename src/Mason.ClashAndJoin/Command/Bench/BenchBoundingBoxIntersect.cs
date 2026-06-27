using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Mason.ClashAndJoin.Command.Bench.Misc;
using Mason.Core;
using Mason.Geometry;

namespace Mason.ClashAndJoin.Command.Bench;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class BenchBoundingBoxIntersect() : AbsCommand(false)
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<Element> elements =
        [
            .. Selection.PickObjects(ObjectType.Element).Select(Doc.GetElement),
        ];
        List<ElementId> elementIds = elements.ConvertAll(e => e.Id);
        long timeMasonProxy = MethodMasonProxy(elementIds);
        long timeMasonExtension = MethodMasonExtension(elements);
        long timeRevit = MethodRevit(elementIds);

        TaskDialog.Show(
            "BoundingBox Intersect Benchmark",
            $"Mason Proxy time: {timeMasonProxy / 1000.0}s\n"
                + $"Mason Extension time: {timeMasonExtension / 1000.0}s\n"
                + $"Revit time: {timeRevit / 1000.0}s"
        );
    }

    private static long MethodMasonProxy(List<ElementId> elements)
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        List<ProxyElement> proxyelements = elements.ConvertAll(e => new ProxyElement(e, true));
        for (int i = 0; i < elements.Count - 1; i++)
        {
            for (int j = i + 1; j < elements.Count; j++)
            {
                BoundingBox.IsIntersect(
                    ref proxyelements[i].CachedBBox,
                    ref proxyelements[j].CachedBBox
                );
            }
        }
        watch.Stop();
        Log.Info($"Mason Proxy BoundingBox Intersect time: {watch.ElapsedMilliseconds}ms");
        return watch.ElapsedMilliseconds;
    }

    private static long MethodMasonExtension(List<Element> elements)
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        List<BoundingBoxXYZ> bboxs = elements.ConvertAll(e => e.get_BoundingBox(ActiveView));
        for (int i = 0; i < elements.Count - 1; i++)
        {
            Element e1 = elements[i];
            for (int j = i + 1; j < elements.Count; j++)
            {
                Element e2 = elements[j];
                BoundingBoxXYZUtils.IsIntersect(bboxs[i], bboxs[j]);
            }
        }
        watch.Stop();
        Log.Info($"Mason Extension BoundingBox Intersect time: {watch.ElapsedMilliseconds}ms");
        return watch.ElapsedMilliseconds;
    }

    private static long MethodRevit(List<ElementId> elementIds)
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < elementIds.Count - 2; i++)
        {
            Element e1 = Doc.GetElement(elementIds[i]);
            BoundingBoxXYZ bbox1 = e1.get_BoundingBox(ActiveView);
            BoundingBoxIntersectsFilter filter = new(new Outline(bbox1.Min, bbox1.Max));
            new FilteredElementCollector(Doc, elementIds.GetRange(i + 1, elementIds.Count - i - 1))
                .WherePasses(filter)
                .ToElements();
        }
        watch.Stop();
        Log.Info($"Revit BoundingBox Intersect time: {watch.ElapsedMilliseconds}s");
        return watch.ElapsedMilliseconds;
    }
}
