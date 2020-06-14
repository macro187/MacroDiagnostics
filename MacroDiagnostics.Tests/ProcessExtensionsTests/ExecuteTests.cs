using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    [TestClass]
    public class ExecuteTests : ProcessExtensionsTest
    {

        [TestMethod]
        public void Succeeds()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(true, true, null, TestExe));
        }


        [TestMethod]
        public void Returns_Correct_ExitCode()
        {
            Assert.AreEqual(
                123,
                ProcessExtensions.Execute(true, true, null, TestExe, "123"));
        }

    }
}
