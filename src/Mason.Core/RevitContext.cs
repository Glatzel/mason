using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Mason.Core;

public static class RevitContext
{
    public static void Init(UIApplication uiApp)
    {
        _uiApp = uiApp;
    }

    private static UIApplication _uiApp;

    public static UIApplication UIApp
    {
        get { return _uiApp; }
    }

    public static UIDocument UIDoc
    {
        get { return UIApp.ActiveUIDocument; }
    }

    public static Document Doc
    {
        get { return UIApp.ActiveUIDocument.Document; }
    }

    public static View ActiveView
    {
        get { return UIApp.ActiveUIDocument.ActiveView; }
    }

    public static Selection Selection
    {
        get { return UIApp.ActiveUIDocument.Selection; }
    }
}

public class AbsContext
{
    protected static UIApplication UIApp
    {
        get { return RevitContext.UIApp; }
    }

    protected static UIDocument UIDoc
    {
        get { return RevitContext.UIApp.ActiveUIDocument; }
    }

    protected static Document Doc
    {
        get { return RevitContext.UIApp.ActiveUIDocument.Document; }
    }

    protected static View ActiveView
    {
        get { return RevitContext.UIApp.ActiveUIDocument.ActiveView; }
    }

    protected static Selection Selection
    {
        get { return RevitContext.UIApp.ActiveUIDocument.Selection; }
    }
}
