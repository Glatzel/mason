using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Mason.Core;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public abstract class AbsCommand(bool autoTransaction = false) : AbsContext, IExternalCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    private readonly bool AutoTransaction = autoTransaction;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        RevitContext.Init(commandData.Application);
        Log.Info($"Document: {Doc.Title}|Start Execute Command: {GetType().FullName}");
        try
        {
            if (AutoTransaction)
            {
                using Transaction ts = new(Doc, GetType().Name);
                ts.Start();
                CommandBody();
                ts.Commit();
            }
            else
            {
                CommandBody();
            }
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
        {
            Log.Warn($"{GetType().FullName}: {e}");
        }
        catch (Exception e)
        {
            Log.Error($"{GetType().FullName}: {e}");
            TaskDialog.Show(GetType().FullName, e.Message);
        }
        Log.Info($"Document: {Doc.Title}|Finish Execute Command: {GetType().FullName}");
        return Result.Succeeded;
    }

    public abstract void CommandBody();
}
