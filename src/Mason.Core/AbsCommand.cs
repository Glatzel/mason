using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Mason.Core;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public abstract class AbsCommand(bool autoTransaction = false) : AbsContext, IExternalCommand
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
    private readonly bool _autoTransaction = autoTransaction;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        RevitContext.Init(commandData.Application);

        string commandName = GetType().FullName ?? "UnknownCommand";
        string documentTitle = Doc?.Title ?? "NoDocument";

        Log.Info($"Document: {documentTitle} | Start Execute Command: {commandName}");

        try
        {
            if (_autoTransaction)
            {
                using Transaction tx = new(Doc, commandName);
                tx.Start();

                CommandBody();

                tx.Commit();
                Log.Debug($"Transaction committed for command {commandName}.");
            }
            else
            {
                CommandBody();
            }
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
        {
            Log.Warn($"{commandName}: {ex}");
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            Log.Error($"{commandName}: {ex}");
            TaskDialog.Show(commandName, ex.Message);
            return Result.Failed;
        }
        finally
        {
            Log.Info($"Document: {documentTitle} | Finish Execute Command: {commandName}");
        }

        return Result.Succeeded;
    }

    public abstract void CommandBody();
}
