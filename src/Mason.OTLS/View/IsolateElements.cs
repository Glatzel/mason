using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using Mason.Core;

namespace Mason.OTLS.View;

public class IsolateElements : AbsOperator<object>
{
    public IsolateElements(params Element[] elements)
    {
        elements.ToList().ForEach(e => Ids.Add(e.Id));
    }

    public IsolateElements(params ElementId[] ids)
    {
        Ids.AddRange(ids);
    }

    public List<ElementId> Ids { get; set; } = [];

    public override void Cook()
    {
        UIDoc.ActiveView.IsolateElementsTemporary(Ids);
    }
}
