using Mason.Core;

namespace Mason.OTLS.Graphics;

public class RevertGraphicsOverride : AbsOperator<object>
{
    public override void Cook()
    {
        Core.Utils.Graphics.ResetGraphicsOverride();
    }
}
