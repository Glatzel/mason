using System.Collections.Generic;
using Autodesk.Revit.DB;
using Mason.Core;

namespace Mason.OTLS.Selection;

public class SetSelection : AbsOperator<object>
{
    public SetSelection() { }

    public SetSelection(params Element[] elements)
    {
        foreach (Element element in elements)
        {
            Ids.Add(element.Id);
        }
    }

    public SetSelection(params ElementId[] ids)
    {
        Ids.AddRange(ids);
    }

    public List<ElementId> Ids { get; set; } = [];

    public override void Cook()
    {
        RevitContext.Selection.SetElementIds(Ids);
    }
}
