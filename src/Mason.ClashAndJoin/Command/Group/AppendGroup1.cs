using System.Collections.Generic;
using Mason.Core;

namespace Mason.ClashAndJoin.Command.Group;

/// <summary>
/// Command to append selected elements to GroupCache1.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class AppendGroup1 : AbsCommand
{
    /// <summary>
    /// Logger for this command.
    /// </summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Executes the command to append selected elements to GroupCache1.
    /// </summary>
    public override void CommandBody()
    {
        // Select elements from current selection
        List<ProxyElement> elements = SelectUtils.SelectProxyElements(Selection);

        if (elements.Count == 0)
        {
            Log.Info("No elements selected to append to Group1.");
            return;
        }

        // Ensure GroupCache1 is initialized
        if (SelectUtils.GroupCache1 == null)
        {
            Log.Warn("GroupCache1 is null. Initializing new list.");
            SelectUtils.GroupCache1 = [];
        }

        // Append selected elements
        SelectUtils.GroupCache1.AddRange(elements);

        Log.Info(
            $"Appended {elements.Count} elements to Group1. Current count: {SelectUtils.GroupCache1.Count}."
        );
    }
}
