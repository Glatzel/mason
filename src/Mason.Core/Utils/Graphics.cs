using System;
using Autodesk.Revit.DB;

namespace Mason.Core.Utils;

/// <summary>
/// Utility methods related to Revit graphics and element display overrides.
/// </summary>
public static class Graphics
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Resets graphics overrides of the specified elements in the current active view.
    /// </summary>
    /// <param name="ids">The element IDs whose overrides should be reset.</param>
    public static void ResetGraphicsOverride(params ElementId[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            Log.Warn("No element IDs provided for ResetGraphicsOverride.");
            return;
        }

        View view = RevitContext.ActiveView;
        if (view == null)
        {
            Log.Error("Failed to reset graphics overrides: no active view found.");
            return;
        }

        Document doc = view.Document;
        if (doc == null)
        {
            Log.Error(
                "Failed to reset graphics overrides: active view has no associated document."
            );
            return;
        }

        OverrideGraphicSettings defaultGraphicSettings = new();
        foreach (ElementId id in ids)
        {
            view.SetElementOverrides(id, defaultGraphicSettings);
        }

        Log.Info($"Reset {ids.Length} graphic overrides in active view \"{view.Name}\".");
    }
}
