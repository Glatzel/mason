using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace Mason.ClashAndJoin;

internal static class SelectUtils
{
    internal static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    private static List<ProxyElement> groupCache1 = [];
    private static List<ProxyElement> groupCache2 = [];

    internal static List<ProxyElement> GroupCache1
    {
        get => [.. groupCache1.Distinct()];
        set => groupCache1 = value;
    }

    internal static List<ProxyElement> GroupCache2
    {
        get => [.. groupCache2.Distinct()];
        set => groupCache2 = value;
    }

    public static List<ProxyElement> SelectProxyElements(Selection selection, bool cacheBBox = true)
    {
        List<ProxyElement> elements = selection
            .GetElementIds()
            .ToList()
            .ConvertAll(e => new ProxyElement(e, cacheBBox));
        if (elements.Count == 0)
        {
            elements =
            [
                .. selection
                    .PickObjects(ObjectType.Element)
                    .Select(r => new ProxyElement(r.ElementId, cacheBBox))
                    .Where(i => i.E.Category.CategoryType == CategoryType.Model),
            ];
        }

        Log.Info($"Select {elements.Count} elements.");
        return elements;
    }
}
