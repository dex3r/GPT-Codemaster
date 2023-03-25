using AiProgrammer.Solving;
using OpenAI_API.Chat;

namespace AiProgrammer.Loggingv1;

public interface ISolverProgressLogger
{
    Task LogChatRequest(Conversation conversation);
    Task LogChatResponse(string response);
}