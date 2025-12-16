using Autodesk.Revit.DB;

using Mason.Core;

namespace Mason.OTLS.View;

public class EnableSectionBox : AbsOperator<object>
{
    public override void Cook()
    {
        View3D view = (View3D)UIDoc.ActiveView;
        view.IsSectionBoxActive = true;
    }
}
