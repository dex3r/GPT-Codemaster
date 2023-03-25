namespace AiProgrammer.AiInterface;

public interface ICompletions
{
    Task<string> GetCompletion(string systemMessage, string messageToComplete);
}
