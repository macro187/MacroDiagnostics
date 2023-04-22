using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MacroDiagnostics.Tests.ProcessExtensionsTests
{
    public abstract class ProcessExtensionsTest
    {

        static ProcessExtensionsTest()
        {
            string frameworkMoniker;
            #if NET7_0
            frameworkMoniker = "net7.0";
            #elif NETCOREAPP3_1
            frameworkMoniker = "netcoreapp3.1";
            #elif NET472
            frameworkMoniker = "net472";
            #else
            #error Unrecognised build framework
            #endif

            var exe = "MacroDiagnostics.Tests.TestExe";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exe += ".exe";
            }

            TestExe = 
                Path.GetFullPath(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "..", "..", "..", "..",
                        "MacroDiagnostics.Tests.TestExe", "bin", "Debug",
                        frameworkMoniker,
                        exe));
        }


        protected static string TestExe { get; }

    }
}
