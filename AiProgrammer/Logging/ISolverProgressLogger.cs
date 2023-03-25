using AiProgrammer.Solving;
using OpenAI_API.Chat;

namespace AiProgrammer.Logging;

public interface ISolverProgressLogger
{
    Task LogChatRequest(Conversation conversation);
    Task LogChatResponse(string response);
}
