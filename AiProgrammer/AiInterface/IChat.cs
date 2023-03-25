using AiProgrammer.Solving;

namespace AiProgrammer.AiInterface;

public interface IChat
{
    Task<string> GetResponseForChat(string systemMessage, string chatMessage);
}
