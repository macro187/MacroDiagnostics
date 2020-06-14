using System;
using System.IO;

namespace MacroDiagnostics.Tests.TestExe
{

    class Program
    {

        static int Main(string[] args)
        {
            //
            // Emit working directory
            //
            if (args.Length > 0 && args[0] == "workingdirectory")
            {
                Console.Out.WriteLine(Path.GetFullPath(Environment.CurrentDirectory));
                return 0;
            }

            //
            // Use the first command line argument as the exit code
            //
            else if(args.Length > 0)
            {
                for (int i=0; i<args.Length; i++)
                {
                    Console.Out.WriteLine("arg{0}: {1}", i, args[i]);
                }

                return int.TryParse(args[0], out var exitCode) ? exitCode : 99;
            }

            //
            // Emit stdout-stderr test pattern
            //
            else
            {
                Console.Out.WriteLine("aaa");
                Console.Out.WriteLine("bbb");
                Console.Out.WriteLine("");
                Console.Out.WriteLine("ccc");
                Console.Error.WriteLine("ddd");
                Console.Error.WriteLine("eee");
                Console.Error.WriteLine("");
                Console.Error.WriteLine("fff");
                return 0;
            }
        }

    }
}
