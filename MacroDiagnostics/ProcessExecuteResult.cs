using MacroGuards;

namespace MacroDiagnostics
{

    public class ProcessExecuteResult
    {

        public ProcessExecuteResult(
            string commandLine,
            string standardOutput,
            string errorOutput,
            string combinedOutput,
            int exitCode)
        {
            Guard.NotNull(commandLine, nameof(commandLine));
            Guard.NotNull(standardOutput, nameof(standardOutput));
            Guard.NotNull(errorOutput, nameof(errorOutput));
            Guard.NotNull(combinedOutput, nameof(combinedOutput));

            CommandLine = commandLine;
            StandardOutput = standardOutput;
            ErrorOutput = errorOutput;
            CombinedOutput = combinedOutput;
            ExitCode = exitCode;
        }


        /// <summary>
        /// Command line
        /// </summary>
        ///
        public string CommandLine { get; }


        /// <summary>
        /// Standard output
        /// </summary>
        ///
        public string StandardOutput { get; }


        /// <summary>
        /// Standard error output
        /// </summary>
        ///
        public string ErrorOutput { get; }


        /// <summary>
        /// Interleaved standard output and standard error output
        /// </summary>
        ///
        /// <remarks>
        /// Lines of output from the two streams are not guaranteed to be received interleaved in exactly the same order
        /// as they were generated.
        /// </remarks>
        ///
        public string CombinedOutput { get; }


        /// <summary>
        /// The process' exit code
        /// </summary>
        ///
        public int ExitCode { get; }

    }
}
