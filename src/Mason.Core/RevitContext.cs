using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Mason.Core;

/// <summary>
/// Provides access to commonly used Revit API objects.
/// Must be initialized via <see cref="Init(UIApplication)"/> before use.
/// </summary>
public static class RevitContext
{
    private static UIApplication _uiApp;

    /// <summary>
    /// Initializes the Revit context.
    /// Must be called once from a valid Revit entry point (e.g. IExternalCommand.Execute).
    /// </summary>
    public static void Init(UIApplication uiApp)
    {
        _uiApp = uiApp ?? throw new ArgumentNullException(nameof(uiApp));
    }

    /// <summary>
    /// The current Revit application instance.
    /// </summary>
    public static UIApplication UIApp
    {
        get
        {
            if (_uiApp == null)
            {
                throw new InvalidOperationException(
                    "RevitContext has not been initialized. Call Init() first."
                );
            }

            return _uiApp;
        }
    }

    /// <summary>
    /// The current active UIDocument.
    /// </summary>
    public static UIDocument UIDoc
    {
        get
        {
            UIDocument doc = UIApp.ActiveUIDocument;
            return doc ?? throw new InvalidOperationException("No active document in Revit.");
        }
    }

    /// <summary>
    /// The current Revit document.
    /// </summary>
    public static Document Doc
    {
        get { return UIDoc.Document; }
    }

    /// <summary>
    /// The current active view.
    /// </summary>
    public static View ActiveView
    {
        get { return UIDoc.ActiveView; }
    }

    /// <summary>
    /// The current selection object.
    /// </summary>
    public static Selection Selection
    {
        get { return UIDoc.Selection; }
    }
}

/// <summary>
/// Base class providing convenient access to Revit context objects.
/// </summary>
public abstract class AbsContext
{
    protected static UIApplication UIApp => RevitContext.UIApp;
    protected static UIDocument UIDoc => RevitContext.UIDoc;
    protected static Document Doc => RevitContext.Doc;
    protected static View ActiveView => RevitContext.ActiveView;
    protected static Selection Selection => RevitContext.Selection;
}
