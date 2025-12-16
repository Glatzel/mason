using System;

namespace Mason.Command.DevTool;

/// <summary>
/// A test command that intentionally throws an exception to verify error handling.
/// </summary>
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ThrowError : Core.AbsCommand
{
    /// <summary>
    /// Main execution body of the command.
    /// </summary>
    /// <exception cref="ArithmeticException">Always thrown to test error handling.</exception>
    public override void CommandBody()
    {
        // Intentionally throw an exception for testing purposes
        throw new ArithmeticException("Test throw error.");
    }
}
