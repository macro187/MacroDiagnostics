using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroDiagnostics.Tests
{

    [TestClass]
    public class ProcessExtensionsTests
    {

        static readonly string TestExe;


        static ProcessExtensionsTests()
        {
            string frameworkMoniker;
            #if NET472
            frameworkMoniker = "net472";
            #elif NETCOREAPP3_1
            frameworkMoniker = "netcoreapp3.1";
            #else
            #error Unrecognised build framework
            #endif

            TestExe = 
                Path.GetFullPath(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "..", "..", "..", "..",
                        "MacroDiagnostics.Tests.TestExe", "bin", "Debug",
                        frameworkMoniker,
                        "MacroDiagnostics.Tests.TestExe.exe"));
        }


        [TestMethod]
        public void Execute_Succeeds()
        {
            Assert.AreEqual(
                0,
                ProcessExtensions.Execute(true, true, null, TestExe));
        }


        [TestMethod]
        public void Execute_Returns_Correct_ExitCode()
        {
            Assert.AreEqual(
                123,
                ProcessExtensions.Execute(true, true, null, TestExe, "123"));
        }


        [TestMethod]
        public void ExecuteCaptured_Passes_Arguments_Correctly()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "a b c", "d", "\"e f\"");
            Assert.IsTrue(result.StandardOutput.Contains("arg0: a b c"));
            Assert.IsTrue(result.StandardOutput.Contains("arg1: d"));
            Assert.IsTrue(result.StandardOutput.Contains("arg2: e f"));
        }


        [TestMethod]
        public void ExecuteCaptured_Captures_CommandLine()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.CommandLine));
        }


        [TestMethod]
        public void ExecuteCaptured_Captures_Output_Correctly()
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
        public void ExecuteCaptured_Uses_Current_WorkingDirectory_By_Default()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "workingdirectory");
            Assert.AreEqual(
                Path.GetFullPath(Environment.CurrentDirectory),
                result.StandardOutput.Trim());
        }


        [TestMethod]
        public void ExecuteCaptured_Uses_Specified_WorkingDirectory()
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
        public void ExecuteCaptured_Relative_WorkingDirectory_Fails()
        {
            ProcessExtensions.ExecuteCaptured(true, true, "relative\\path", TestExe, "workingdirectory");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteCaptured_Non_Existent_WorkingDirectory_Fails()
        {
            ProcessExtensions.ExecuteCaptured(true, true, "C:\\does\\not\\exist", TestExe, "workingdirectory");
        }

    }
}
