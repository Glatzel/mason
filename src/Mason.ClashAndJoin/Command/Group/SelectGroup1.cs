using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

/// <summary>
/// Command to select a group of elements and cache them for later group operations.
/// Stores the selected elements in <see cref="SelectUtils.GroupCache1"/>.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class SelectGroup1 : AbsCommand
{
    /// <summary>
    /// Logger for the command.
    /// </summary>
    internal static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Executes the group selection and caches the selected elements.
    /// </summary>
    public override void CommandBody()
    {
        System.Collections.Generic.List<ProxyElement> selectedElements =
            SelectUtils.SelectProxyElements(Selection) ?? [];
        SelectUtils.GroupCache1 = selectedElements;

        Log.Info($"Selected {selectedElements.Count} elements and cached for group operations.");
    }
}
