using System.IO;
using System.Reflection;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    public abstract class ProcessExtensionsTest
    {

        static ProcessExtensionsTest()
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


        protected static string TestExe { get; }

    }
}
