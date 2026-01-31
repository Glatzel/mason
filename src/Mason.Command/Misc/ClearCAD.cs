using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Mason.Core;

namespace Mason.Command.Misc;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ClearCAD() : AbsCommand(true)
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public override void CommandBody()
    {
        List<ElementId> cadFiles =
        [
            .. new FilteredElementCollector(Doc)
                .OfClass(typeof(ImportInstance))
                .Cast<ImportInstance>()
                .Where(i => !i.IsLinked)
                .Select(i => i.Id),
        ];
        Log.Info($"Find {cadFiles.Count} Cad Files.");
        if (cadFiles.Count > 0)
        {
            Doc.Delete(cadFiles);
        }
    }
}
