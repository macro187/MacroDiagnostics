using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MacroDiagnostics.Tests.LogicalOperationTests
{
    [TestClass]
    public class StartTests
    {

        [TestMethod]
        public void Pushes_And_Pops_Correctly()
        {
            int initialCount = Trace.CorrelationManager.LogicalOperationStack.Count;
            string op = "op";

            using (LogicalOperation.Start(op))
            {
                Trace.CorrelationManager.LogicalOperationStack.Count.ShouldBe(initialCount + 1);
                Trace.CorrelationManager.LogicalOperationStack.Peek().ShouldBe(op);
            }

            Trace.CorrelationManager.LogicalOperationStack.Count.ShouldBe(initialCount);
        }

    }
}
