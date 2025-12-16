using Autodesk.Revit.DB;

using Mason.Core;

namespace Mason.OTLS.View;

public class DisableSectionBox : AbsOperator<object>
{
    public override void Cook()
    {
        View3D view = (View3D)UIDoc.ActiveView;
        view.IsSectionBoxActive = false;
    }
}
