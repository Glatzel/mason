using Autodesk.Revit.DB;
using Mason.Core;

namespace Mason.OTLS.View;

public class DisableTemporayHideIsolate : AbsOperator<object>
{
    public override void Cook()
    {
        if (ActiveView.IsTemporaryHideIsolateActive())
            ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
    }
}
