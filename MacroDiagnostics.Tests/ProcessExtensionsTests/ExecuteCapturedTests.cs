using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    [TestClass]
    public class ExecuteCapturedTests : ProcessExtensionsTest
    {

        [TestMethod]
        public void Passes_Arguments_Correctly()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "a b c", "d", "\"e f\"");
            Assert.IsTrue(result.StandardOutput.Contains("arg0: a b c"));
            Assert.IsTrue(result.StandardOutput.Contains("arg1: d"));
            Assert.IsTrue(result.StandardOutput.Contains("arg2: e f"));
        }


        [TestMethod]
        public void Captures_CommandLine()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.CommandLine));
        }


        [TestMethod]
        public void Captures_Output_Correctly()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe);

            Assert.AreEqual(
                string.Join(Environment.NewLine,
                    "aaa",
                    "bbb",
                    "ccc"),
                result.StandardOutput.Trim());

            Assert.AreEqual(
                string.Join(Environment.NewLine,
                    "ddd",
                    "eee",
                    "fff"),
                result.ErrorOutput.Trim());

            Assert.AreEqual(
                string.Join(Environment.NewLine,
                    "aaa",
                    "bbb",
                    "ccc",
                    "ddd",
                    "eee",
                    "fff"),
                string.Join(Environment.NewLine,
                    result.CombinedOutput
                        .Trim()
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                        .OrderBy(s => s)));

            Assert.AreEqual(0, result.ExitCode);
        }


        [TestMethod]
        public void Uses_Current_WorkingDirectory_By_Default()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "workingdirectory");
            Assert.AreEqual(
                Path.GetFullPath(Environment.CurrentDirectory),
                result.StandardOutput.Trim());
        }


        [TestMethod]
        public void Uses_Specified_WorkingDirectory()
        {
            var here = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var testWorkingDirectory = Path.Combine(here, "testworkingdirectory");
            Directory.CreateDirectory(testWorkingDirectory);
            var result =
                ProcessExtensions.ExecuteCaptured(true, true, testWorkingDirectory, TestExe, "workingdirectory");
            Assert.AreEqual(
                testWorkingDirectory,
                result.StandardOutput.Trim());
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Relative_WorkingDirectory_Fails()
        {
            ProcessExtensions.ExecuteCaptured(true, true, "relative\\path", TestExe, "workingdirectory");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Non_Existent_WorkingDirectory_Fails()
        {
            ProcessExtensions.ExecuteCaptured(true, true, "C:\\does\\not\\exist", TestExe, "workingdirectory");
        }

    }
}
