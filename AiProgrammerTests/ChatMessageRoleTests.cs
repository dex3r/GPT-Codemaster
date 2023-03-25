using NUnit.Framework;
using OpenAI_API.Chat;

namespace AiProgrammerTests;

public class ChatMessageRoleTests
{
    [Test]
    public void ImplicitOp()
    {
        string role2 = ChatMessageRole.User;
    }
}
