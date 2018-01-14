using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MacroDiagnostics.Tests
{


[TestClass]
public class
LogicalOperationTests
{


[TestMethod]
public void
Start_Pushes_And_Pops_Correctly()
{
    int initialCount = Trace.CorrelationManager.LogicalOperationStack.Count;
    string op = "op";
    using (LogicalOperation.Start(op))
    {
        Assert.AreEqual(initialCount + 1, Trace.CorrelationManager.LogicalOperationStack.Count);
        Assert.AreEqual(op, Trace.CorrelationManager.LogicalOperationStack.Peek() as string);
    }
    Assert.AreEqual(initialCount, Trace.CorrelationManager.LogicalOperationStack.Count);
}


}
}
