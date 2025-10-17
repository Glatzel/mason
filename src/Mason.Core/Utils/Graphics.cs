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
    /// <exception cref="ArgumentNullException">Thrown if no element IDs are provided.</exception>
    /// <exception cref="InvalidOperationException">Thrown if there is no active view or associated document.</exception>
    public static void ResetGraphicsOverride(params ElementId[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            Log.Warn("No element IDs provided for ResetGraphicsOverride.");
            throw new ArgumentNullException(nameof(ids), "No element IDs provided.");
        }

        View view = RevitContext.ActiveView;
        if (view == null)
        {
            Log.Warn("Failed to reset graphics overrides: no active view found.");
            throw new InvalidOperationException(
                "No active view found in the current Revit context."
            );
        }

        Document doc = view.Document;
        if (doc == null)
        {
            Log.Warn("Failed to reset graphics overrides: active view has no associated document.");
            throw new InvalidOperationException("Active view has no associated document.");
        }

        OverrideGraphicSettings defaultGraphicSettings = new();
        foreach (ElementId id in ids)
        {
            view.SetElementOverrides(id, defaultGraphicSettings);
        }

        Log.Info($"Reset {ids.Length} graphic overrides in active view \"{view.Name}\".");
    }
}
