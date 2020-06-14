using System;
using MacroGuards;

namespace MacroDiagnostics
{
    public class ProcessExecuteException : Exception
    {

        public ProcessExecuteException(ProcessExecuteResult result)
        {
            Guard.NotNull(result, nameof(result));
            Result = result;
        }


        public ProcessExecuteResult Result { get; }

    }
}
