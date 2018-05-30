using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MacroGuards;
using MacroSystem;


namespace
MacroDiagnostics
{


public static class
ProcessExtensions
{


static
ProcessExtensions()
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


static string
ExecuteAnyWrapper { get; }


static IEnumerable<string>
ExecuteAnyWrapperArgs { get; }


/// <summary>
/// Run any kind of program, possibly from the system path
/// </summary>
///
/// <remarks>
/// Compared to <see cref="Execute"/>, this method can run more types of programs (e.g. batch files on Windows) and can
/// find them on the system path.  It uses <c>cmd /c</c> on Windows systems and <c>env</c> on Unix systems.
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// The exit code from the program
/// </returns>
///
public static int
ExecuteAny(
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
/// Compared to <see cref="Execute"/>, this method can run more types of programs (e.g. batch files on Windows) and can
/// find them on the system path.  It uses <c>cmd /c</c> on Windows systems and <c>env</c> on Unix systems.
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
/// </returns>
///
public static ProcessExecuteResult
ExecuteAnyCaptured(
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
/// If the current process is running under a .NET framework "driver" program such as <c>dotnet</c> or <c>mono</c>, the
/// same one will be used to run the program.
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// The exit code from the program
/// </returns>
///
public static int
ExecuteDotnet(
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
/// If the current process is running under a .NET framework "driver" program such as <c>dotnet</c> or <c>mono</c>, the
/// same one will be used to run the specified program.
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
/// </returns>
///
public static ProcessExecuteResult
ExecuteDotnetCaptured(
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// The exit code from the program
/// </returns>
///
public static int
Execute(
    bool echoCommandLine,
    bool echoOutput,
    string workingDirectory,
    string fileName,
    params string[] arguments)
{
    Action<string> onCommandLine = null;
    Action<string> onStandardOutput = null;
    Action<string> onErrorOutput = null;
    if (echoCommandLine)
    {
        onCommandLine = s => Console.Error.WriteLine(s);
    }
    if (echoOutput)
    {
        onStandardOutput = s => Console.Out.WriteLine(s);
        onErrorOutput = s => Console.Error.WriteLine(s);
    }
    return Execute(onCommandLine, onStandardOutput, onErrorOutput, workingDirectory, fileName, arguments);
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
/// Arguments to pass to the program.  Those containing space characters will be quoted, unless they also contain quote
/// characters in which case they are assumed to be pre-quoted.
/// </param>
///
/// <returns>
/// A <see cref="ProcessExecuteResult"/> containing the program's output and exit code
/// </returns>
///
public static ProcessExecuteResult
ExecuteCaptured(
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

    Action<string> onCommandLine = s => {
        if (echoCommandLine) Console.Error.WriteLine(s);
        commandLine = s;
    };

    Action<string> onStandardOutput = s => {
        if (echoOutput) Console.Out.WriteLine(s);
        combinedOutput.AppendLine(s);
        standardOutput.AppendLine(s);
    };

    Action<string> onErrorOutput = s => {
        if (echoOutput) Console.Error.WriteLine(s);
        combinedOutput.AppendLine(s);
        errorOutput.AppendLine(s);
    };

    int exitCode = Execute(onCommandLine, onStandardOutput, onErrorOutput, workingDirectory, fileName, arguments);

    return new ProcessExecuteResult(
        commandLine,
        standardOutput.ToString(),
        errorOutput.ToString(),
        combinedOutput.ToString(),
        exitCode);
}


static int
Execute(
    Action<string> onCommandLine,
    Action<string> onStandardOutput,
    Action<string> onErrorOutput,
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
            
    return Execute(onCommandLine, onStandardOutput, onErrorOutput, startInfo);
}


static int
Execute(
    Action<string> onCommandLine,
    Action<string> onStandardOutput,
    Action<string> onErrorOutput,
    ProcessStartInfo startInfo)
{
    onCommandLine = onCommandLine ?? (_ => {});
    onStandardOutput = onStandardOutput ?? (_ => {});
    onErrorOutput = onErrorOutput ?? (_ => {});
    Guard.NotNull(startInfo, nameof(startInfo));

    var commandLine = startInfo.FileName;
    if (commandLine.Contains(" "))
        commandLine = string.Concat("\"", commandLine, "\"");
    if (startInfo.Arguments.Length > 0)
        commandLine = string.Concat(commandLine, " ", startInfo.Arguments);
            
    using (var proc = new Process())
    {
        proc.StartInfo = startInfo;

        bool exited = false;
        object outputLock = new object();

        proc.StartInfo.RedirectStandardOutput = true;
        proc.OutputDataReceived += (_,e) => {
            lock (outputLock)
            {
                onStandardOutput(e.Data ?? "");
            }
        };

        proc.StartInfo.RedirectStandardError = true;
        proc.ErrorDataReceived += (_,e) => {
            lock (outputLock)
            {
                onErrorOutput(e.Data ?? "");
            }
        };

        proc.EnableRaisingEvents = true;
        proc.Exited += (_,__) => exited = true;

        onCommandLine(commandLine);

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        while (!exited) Thread.Yield();

        return proc.ExitCode;
    }
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
