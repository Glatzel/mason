using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Mason.Core;

/// <summary>
/// Manages and executes queued <see cref="IRevitOperatorCook"/> instances
/// inside a Revit-safe <see cref="ExternalEvent"/> context.
/// </summary>
public class OperatorManager : IExternalEventHandler
{
    private readonly ExternalEvent _revitEvent;
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Queue of operators waiting to be executed.
    /// </summary>
    protected Queue<IRevitOperatorCook> Operators { get; } = new();

    public OperatorManager()
    {
        _revitEvent = ExternalEvent.Create(this);
    }

    /// <inheritdoc />
    public void Execute(UIApplication app)
    {
        try
        {
            RevitContext.Init(app);
            Log.Info("Executing OperatorManager event handler.");
            CookOperators();
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
        {
            Log.Warn($"OperatorManager canceled: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception in OperatorManager.Execute");
            TaskDialog.Show(GetName(), ex.Message);
        }
    }

    /// <inheritdoc />
    public string GetName()
    {
        return GetType().FullName ?? nameof(OperatorManager);
    }

    /// <summary>
    /// Raises the <see cref="ExternalEvent"/>, causing queued operators to run in Revit's main thread.
    /// </summary>
    public void Raise()
    {
        Log.Debug($"Raising OperatorManager with {Operators.Count} operator(s) queued.");
        _revitEvent.Raise();
    }

    /// <summary>
    /// Adds operators to the execution queue.
    /// </summary>
    public void Enqueue(params IRevitOperatorCook[] ops)
    {
        foreach (IRevitOperatorCook op in ops)
        {
            Operators.Enqueue(op);
            Log.Trace($"Enqueued operator: {op.GetType().FullName}");
        }
    }

    /// <summary>
    /// Executes all queued operators inside a single Revit transaction.
    /// </summary>
    private void CookOperators()
    {
        if (Operators.Count == 0)
        {
            Log.Debug("No operators to execute.");
            return;
        }

        Document doc = RevitContext.Doc;
        using Transaction tx = new(doc, GetType().Name);
        tx.Start();

        while (Operators.Count > 0)
        {
            IRevitOperatorCook op = Operators.Dequeue();
            try
            {
                Log.Debug($"Executing operator: {op.GetType().FullName}");
                op.Cook();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Operator failed: {op.GetType().FullName}");
            }
        }

        tx.Commit();
        Log.Info("All queued operators executed successfully.");
    }
}
