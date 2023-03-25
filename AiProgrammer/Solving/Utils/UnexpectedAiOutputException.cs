using AiProgrammer.Extensions;

namespace AiProgrammer.Solving.Utils;

public class UnexpectedAiOutputException : Exception
{
    public UnexpectedAiOutputException(string message, string fullAiOutput)
    : base($"{message}. First 50 characters of output: '{fullAiOutput.SubstringSafe(0, 50)}'")
    {
    }
}
