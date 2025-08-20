using System.Collections.Generic;
using Autodesk.Revit.DB;
using Mason.Core;

namespace Mason.OTLS.Selection;

public class SetSelction : AbsOperator<object>
{
    public SetSelction() { }

    public SetSelction(params Element[] elements)
    {
        foreach (Element element in elements)
        {
            Ids.Add(element.Id);
        }
    }

    public SetSelction(params ElementId[] ids)
    {
        Ids.AddRange(ids);
    }

    public List<ElementId> Ids { get; set; } = [];

    public override void Cook()
    {
        RevitContext.Selection.SetElementIds(Ids);
    }
}
