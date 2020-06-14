using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MacroGuards;
using MacroSystem;

namespace MacroDiagnostics
{

    public static class ProcessExtensions
    {

        static ProcessExtensions()
        {
            if (EnvironmentExtensions.IsWindows)
            {
                ExecuteAnyWrapper = "cmd";
                ExecuteAnyWrapperArgs = new[]{ "/c" };
            }
            else
            {
                ExecuteAnyWrapper = "env";
                ExecuteAnyWrapperArgs = new string[0];
            }
        }


        static string ExecuteAnyWrapper { get; }
        static IEnumerable<string> ExecuteAnyWrapperArgs { get; }


        /// <summary>
        /// Run any kind of program, possibly from the system path
        /// </summary>
        ///
        /// <remarks>
        /// Compared to <see cref="Execute"/>, this method can run more types of programs (e.g. batch files on Windows)
        /// and can find them on the system path.  It uses <c>cmd /c</c> on Windows systems and <c>env</c> on Unix
        /// systems.
        /// </remarks>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Name of the program to run
        /// - OR -
        /// Full path to the program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// The exit code from the program
        /// </returns>
        ///
        public static int ExecuteAny(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            arguments = Enumerable.Empty<string>()
                .Concat(ExecuteAnyWrapperArgs)
                .Concat(new[]{ fileName })
                .Concat(arguments)
                .ToArray();
            fileName = ExecuteAnyWrapper;
            return Execute(echoCommandLine, echoOutput, workingDirectory, fileName, arguments);
        }


        /// <summary>
        /// Run any kind of program, possibly from the system path, capturing its output
        /// </summary>
        ///
        /// <remarks>
        /// Compared to <see cref="Execute"/>, this method can run more types of programs (e.g. batch files on Windows)
        /// and can find them on the system path.  It uses <c>cmd /c</c> on Windows systems and <c>env</c> on Unix
        /// systems.
        /// </remarks>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Filename of the program to run, if it is on the system <c>path</c>
        /// - OR -
        /// Full path to the program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
        /// </returns>
        ///
        public static ProcessExecuteResult ExecuteAnyCaptured(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            arguments = Enumerable.Empty<string>()
                .Concat(ExecuteAnyWrapperArgs)
                .Concat(new[]{ fileName })
                .Concat(arguments)
                .ToArray();
            fileName = ExecuteAnyWrapper;
            return ExecuteCaptured(echoCommandLine, echoOutput, workingDirectory, fileName, arguments);
        }


        /// <summary>
        /// Run a .NET program
        /// </summary>
        ///
        /// <remarks>
        /// If the current process is running under a .NET framework "driver" program such as <c>dotnet</c> or
        /// <c>mono</c>, the same one will be used to run the program.
        /// </remarks>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Full path to the .NET program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// The exit code from the program
        /// </returns>
        ///
        public static int ExecuteDotnet(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            if (EnvironmentExtensions.DotnetProgram != null)
            {
                arguments = new[]{ fileName }.Concat(arguments).ToArray();
                fileName = EnvironmentExtensions.DotnetProgram;
            }

            return Execute(echoCommandLine, echoOutput, workingDirectory, fileName, arguments);
        }


        /// <summary>
        /// Run a .NET program, capturing its output
        /// </summary>
        ///
        /// <remarks>
        /// If the current process is running under a .NET framework "driver" program such as <c>dotnet</c> or
        /// <c>mono</c>, the same one will be used to run the specified program.
        /// </remarks>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Full path to the .NET program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
        /// </returns>
        ///
        public static ProcessExecuteResult ExecuteDotnetCaptured(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            if (EnvironmentExtensions.DotnetProgram != null)
            {
                arguments = new[]{ fileName }.Concat(arguments).ToArray();
                fileName = EnvironmentExtensions.DotnetProgram;
            }

            return ExecuteCaptured(echoCommandLine, echoOutput, workingDirectory, fileName, arguments);
        }


        /// <summary>
        /// Run a native binary executable program
        /// </summary>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Full path to the program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// The exit code from the program
        /// </returns>
        ///
        public static int Execute(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            return ExecuteCaptured(echoCommandLine, echoOutput, workingDirectory, fileName, arguments).ExitCode;
        }


        /// <summary>
        /// Run a native binary executable program and read its output as it runs
        /// </summary>
        ///
        /// <param name="successExitCodes">
        /// List of exit code(s) to consider successful
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Full path to the program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// Lines of standard output as they are produced by the program
        /// </returns>
        ///
        /// <exception cref="ProcessExecuteException">
        /// The process exited with an exit code not in <paramref name="successExitCodes"/>
        /// </exception>
        ///
        public static IEnumerable<string> ExecuteAndRead(
            IReadOnlyCollection<int> successExitCodes,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            successExitCodes = successExitCodes ?? new[]{ 0 };

            object locker = new object();
            string commandLine = "";
            var outputQueue = new Queue<string>();
            var standardOutput = new StringBuilder();
            var errorOutput = new StringBuilder();
            var combinedOutput = new StringBuilder();
            int? exitCode = null;

            void OnCommandLine(string s)
            {
                lock (locker)
                {
                    commandLine = s;
                }
            }

            void OnStandardOutput(string s)
            {
                lock (locker)
                {
                    standardOutput.AppendLine(s);
                    combinedOutput.AppendLine(s);
                    outputQueue.Enqueue(s);
                    Monitor.PulseAll(locker);
                }
            }

            void OnErrorOutput(string s)
            {
                lock (locker)
                {
                    combinedOutput.AppendLine(s);
                    errorOutput.AppendLine(s);
                }
            }

            void OnExited(int i)
            {
                lock (locker)
                {
                    exitCode = i;
                    Monitor.PulseAll(locker);
                }
            }

            //
            // Start the process...
            //
            Execute(OnCommandLine, OnStandardOutput, OnErrorOutput, OnExited, workingDirectory, fileName, arguments);

            //
            // ...and while it runs, repeatedly...
            //
            while (true)
            {
                bool shouldThrow = false;
                bool shouldReturn = false;
                string line = null;

                lock (locker)
                {
                    //
                    // ...wait for it to produce some output or exit...
                    //
                    while (exitCode == null && outputQueue.Count == 0) Monitor.Wait(locker);

                    //
                    // ...decide what to do about it...
                    //
                    if (exitCode.HasValue && !successExitCodes.Contains(exitCode.Value))
                    {
                        shouldThrow = true;
                    }
                    else if (exitCode.HasValue && outputQueue.Count == 0)
                    {
                        shouldReturn = true;
                    }
                    else
                    {
                        line = outputQueue.Count > 0 ? outputQueue.Dequeue() : null;
                    }
                }

                //
                // ...and do it
                //
                if (shouldThrow)
                {
                    throw
                        new ProcessExecuteException(
                            new ProcessExecuteResult(
                                commandLine,
                                standardOutput.ToString(),
                                errorOutput.ToString(),
                                combinedOutput.ToString(),
                                exitCode.Value));
                }

                if (shouldReturn)
                {
                    break;
                }

                if (line != null)
                {
                    yield return line;
                }
            }
        }


        /// <summary>
        /// Run a native binary executable program, capturing its output
        /// </summary>
        ///
        /// <param name="echoCommandLine">
        /// Echo full command line to stderr?
        /// </param>
        ///
        /// <param name="echoOutput">
        /// Echo stdout and stderr output?
        /// </param>
        ///
        /// <param name="workingDirectory">
        /// Absolute path to existent working directory
        /// - OR -
        /// <c>null</c> to use the current process's working directory
        /// </param>
        ///
        /// <param name="fileName">
        /// Full path to the native binary executable program to run
        /// </param>
        ///
        /// <param name="arguments">
        /// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also
        /// contain quote characters in which case they are assumed to be pre-quoted.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
        /// </returns>
        ///
        public static ProcessExecuteResult ExecuteCaptured(
            bool echoCommandLine,
            bool echoOutput,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            string commandLine = "";
            var combinedOutput = new StringBuilder();
            var standardOutput = new StringBuilder();
            var errorOutput = new StringBuilder();
            int? exitCode = null;
            object locker = new object();

            Action<string> onCommandLine = s => {
                lock (locker)
                {
                    if (echoCommandLine) Console.Error.WriteLine(s);
                    commandLine = s;
                }
            };

            Action<string> onStandardOutput = s => {
                lock (locker)
                {
                    if (echoOutput) Console.Out.WriteLine(s);
                    combinedOutput.AppendLine(s);
                    standardOutput.AppendLine(s);
                }
            };

            Action<string> onErrorOutput = s => {
                lock (locker)
                {
                    if (echoOutput) Console.Error.WriteLine(s);
                    combinedOutput.AppendLine(s);
                    errorOutput.AppendLine(s);
                }
            };

            Action<int> onExited = i => {
                lock (locker)
                {
                    exitCode = i;
                    Monitor.PulseAll(locker);
                }
            };

            Execute(onCommandLine, onStandardOutput, onErrorOutput, onExited, workingDirectory, fileName, arguments);
            lock (locker) while (exitCode == null) Monitor.Wait(locker);

            return new ProcessExecuteResult(
                commandLine,
                standardOutput.ToString(),
                errorOutput.ToString(),
                combinedOutput.ToString(),
                exitCode.Value);
        }


        static void Execute(
            Action<string> onCommandLine,
            Action<string> onStandardOutput,
            Action<string> onErrorOutput,
            Action<int> onExited,
            string workingDirectory,
            string fileName,
            params string[] arguments)
        {
            onCommandLine = onCommandLine ?? (_ => {});
            onStandardOutput = onStandardOutput ?? (_ => {});
            onErrorOutput = onErrorOutput ?? (_ => {});
            workingDirectory = workingDirectory ?? Path.GetFullPath(Environment.CurrentDirectory);
            if (!Path.IsPathRooted(workingDirectory))
                throw new ArgumentException("Specified working directory is not an absolute path", nameof(workingDirectory));
            if (!Directory.Exists(workingDirectory))
                throw new ArgumentException("Specified working directory doesn't exist", nameof(workingDirectory));
            Guard.Required(fileName, nameof(fileName));
            Guard.NotNull(arguments, nameof(arguments));

            var argumentsString = CombineArguments(arguments);

            var startInfo = new ProcessStartInfo(fileName, argumentsString);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = workingDirectory;
                    
            Execute(onCommandLine, onStandardOutput, onErrorOutput, onExited, startInfo);
        }


        static void Execute(
            Action<string> onCommandLine,
            Action<string> onStandardOutput,
            Action<string> onErrorOutput,
            Action<int> onExited,
            ProcessStartInfo startInfo)
        {
            onCommandLine = onCommandLine ?? (_ => {});
            onStandardOutput = onStandardOutput ?? (_ => {});
            onErrorOutput = onErrorOutput ?? (_ => {});
            onExited = onExited ?? (_ => {});
            Guard.NotNull(startInfo, nameof(startInfo));

            var commandLine = startInfo.FileName;
            if (commandLine.Contains(" "))
                commandLine = string.Concat("\"", commandLine, "\"");
            if (startInfo.Arguments.Length > 0)
                commandLine = string.Concat(commandLine, " ", startInfo.Arguments);
                    
            var proc = new Process();
            proc.StartInfo = startInfo;

            proc.StartInfo.RedirectStandardOutput = true;
            proc.OutputDataReceived += (_,e) => {
                if (e.Data != null) onStandardOutput(e.Data);
            };

            proc.StartInfo.RedirectStandardError = true;
            proc.ErrorDataReceived += (_,e) => {
                if (e.Data != null) onErrorOutput(e.Data);
            };

            proc.EnableRaisingEvents = true;
            proc.Exited += (_,__) => {
                proc.WaitForExit();
                onExited(proc.ExitCode);
                proc.Dispose();
            };

            onCommandLine(commandLine);

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
        }


        static string CombineArguments(IEnumerable<string> args)
        {
            Guard.NotNull(args, nameof(args));

            var quotedArgs = 
                args.Select(s =>
                    s.Contains(" ") && !s.Contains("\"")
                        ? string.Concat("\"", s, "\"")
                        : s);

            return string.Join(" ", quotedArgs);
        }

    }
}
