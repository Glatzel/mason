using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

/// <summary>
/// Command to clear both cached groups of selected elements.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ClearAll() : AbsCommand(false)
{
    /// <summary>
    /// Executes the command to clear both group caches.
    /// </summary>
    public override void CommandBody()
    {
        SelectUtils.GroupCache1 = [];
        SelectUtils.GroupCache2 = [];
    }
}
