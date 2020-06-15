using Shouldly;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    [TestClass]
    public class ExecuteAndReadTests : ProcessExtensionsTest
    {

        [TestMethod]
        public void Yields_Correct_Lines()
        {
            var lines = ProcessExtensions.ExecuteAndRead(null, false, false, null, TestExe).ToList();
            lines.ShouldBe(
                new[]{
                    "aaa",
                    "bbb",
                    "",
                    "ccc",
                });
        }


        [TestMethod]
        public void Throws_ProcessExecuteException_On_Unsuccessful_ExitCode()
        {
            Should.Throw<ProcessExecuteException>(() =>
                ProcessExtensions.ExecuteAndRead(null, false, false, null, TestExe, "99").ToList());
        }

    }
}
