using System;

namespace Mason.Command.DevTool;

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class ThrowError : Core.AbsCommand
{
    public override void CommandBody()
    {
        throw new ArithmeticException("Test throw error.");
    }
}
