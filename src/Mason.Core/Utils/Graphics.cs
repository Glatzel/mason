using Autodesk.Revit.DB;

namespace Mason.Core.Utils;

public static class Graphics
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public static void ResetGraphicsOverride(params ElementId[] ids)
    {
        OverrideGraphicSettings defaultGraphicSettings = new();
        View view = RevitContext.ActiveView;
        foreach (ElementId id in ids)
        {
            view.SetElementOverrides(id, defaultGraphicSettings);
        }
        Log.Info($"Reset {ids.Length} graphic overrides of element in active view `{view.Name}`.");
    }
}
