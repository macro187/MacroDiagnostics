using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    [TestClass]
    public class ExecuteCapturedTests : ProcessExtensionsTest
    {

        string Lines(params string[] lines) =>
            string.Concat(lines.Select(line => line + Environment.NewLine));


        [TestMethod]
        public void Passes_Arguments_Correctly()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "a b c", "d", "\"e f\"");
            result.StandardOutput.ShouldContain("arg0: a b c");
            result.StandardOutput.ShouldContain("arg1: d");
            result.StandardOutput.ShouldContain("arg2: e f");
        }


        [TestMethod]
        public void Captures_CommandLine()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe);
            result.CommandLine.ShouldNotBeNullOrWhiteSpace();
        }


        [TestMethod]
        public void Captures_Output_Correctly()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe);

            result.StandardOutput.ShouldBe(
                Lines(
                    "aaa",
                    "bbb",
                    "",
                    "ccc"));

            result.ErrorOutput.ShouldBe(
                Lines(
                    "ddd",
                    "eee",
                    "",
                    "fff"));

                
            result.CombinedOutput.Trim().Split(new[]{Environment.NewLine}, StringSplitOptions.None)
                .ShouldBe(
                    new[]{
                        "aaa",
                        "bbb",
                        "",
                        "ccc",
                        "ddd",
                        "eee",
                        "",
                        "fff",
                    },
                    ignoreOrder: true);

            result.ExitCode.ShouldBe(0);
        }


        [TestMethod]
        public void Uses_Current_WorkingDirectory_By_Default()
        {
            var result = ProcessExtensions.ExecuteCaptured(true, true, null, TestExe, "workingdirectory");
            result.StandardOutput.Trim().ShouldBe(Path.GetFullPath(Environment.CurrentDirectory));
        }


        [TestMethod]
        public void Uses_Specified_WorkingDirectory()
        {
            var here = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var testWorkingDirectory = Path.Combine(here, "testworkingdirectory");
            Directory.CreateDirectory(testWorkingDirectory);
            var result =
                ProcessExtensions.ExecuteCaptured(true, true, testWorkingDirectory, TestExe, "workingdirectory");
            result.StandardOutput.Trim().ShouldBe(testWorkingDirectory);
        }


        [TestMethod]
        public void Relative_WorkingDirectory_Fails()
        {
            Should.Throw<ArgumentException>(() =>
                ProcessExtensions.ExecuteCaptured(true, true, "relative\\path", TestExe, "workingdirectory"));
        }


        [TestMethod]
        public void Non_Existent_WorkingDirectory_Fails()
        {
            Should.Throw<ArgumentException>(() =>
                ProcessExtensions.ExecuteCaptured(true, true, "C:\\does\\not\\exist", TestExe, "workingdirectory"));
        }

    }
}
