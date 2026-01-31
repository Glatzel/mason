using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Mason.Core;

public class OperatorManager : IExternalEventHandler
{
    private readonly ExternalEvent _revitEvent;

    public OperatorManager()
    {
        _revitEvent = ExternalEvent.Create(this);
    }

    public void Execute(UIApplication app)
    {
        RevitContext.Init(app);
        CookOpertators();
    }

    public string GetName()
    {
        return GetType().Name;
    }

    public void Raise()
    {
        _revitEvent.Raise();
    }

    protected Queue<IRevitOperatorCook> Operators { get; set; } = new();

    private void CookOpertators()
    {
        using Transaction ts = new(RevitContext.Doc, GetType().Name);
        ts.Start();
        while (Operators.Count > 0)
        {
            IRevitOperatorCook op = Operators.Dequeue();
            op.Cook();
        }
        ts.Commit();
    }

    public void Enqueue(params IRevitOperatorCook[] ops)
    {
        foreach (IRevitOperatorCook op in ops)
        {
            Operators.Enqueue(op);
        }
    }
}
