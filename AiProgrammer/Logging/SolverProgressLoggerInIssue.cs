using AiProgrammer.Github;
using AiProgrammer.Solving;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Model;
using OpenAI_API.Chat;

namespace AiProgrammer.Loggingv1;

public class SolverProgressLoggerInIssue : ISolverProgressLogger
{
    private readonly object _sync = new();

    private readonly IGithubContextClientProvider _githubClientProvider;
    private readonly IGithubIssueContextProvider _contextProvider;

    public SolverProgressLoggerInIssue(IGithubContextClientProvider githubClientProvider, IGithubIssueContextProvider contextProvider)
    {
        _githubClientProvider = githubClientProvider;
        _contextProvider = contextProvider;
    }
    
    public async Task LogChatRequest(Conversation conversation)
    {
        string comment = $"# Sending request to the AI using Chat interface\n" +
                         $"## System message:\n" +
                         $"{conversation.Messages.FirstOrDefault(x => x.Role == "system")?.Content}\n" +
                         $"\n" +
                         $"## User Request:\n" +
                         $"{conversation.Messages.FirstOrDefault(x => x.Role == "user")?.Content}\n" +
                         $"\bn";

        lock (_sync)
        {
            //TODO: Instead of logging to Console here, it should log to ILogger in separate class. ILogger should then log to console.
            Console.WriteLine($"Creating a new issue comment:\n{comment}");
        }

        GithubIssueContext context = _contextProvider.Get();
        
        //TODO: This should be logged here always. Decision if this logger is used should be done at DI level
        await _githubClientProvider.Get().Issue.Comment.Create(context.RepoOwnerName, context.RepoName, context.SourceIssue.Number,
            comment);
    }

    public async Task LogChatResponse(string response)
    {
        string comment = $"## AI Response\n" +
                         $"{response}";

        GithubIssueContext context = _contextProvider.Get();
        
        //TODO: This should be logged here always. Decision if this logger is used should be done at DI level
        await context.GitHubClient.Issue.Comment.Create(context.RepoOwnerName, context.RepoName, context.SourceIssue.Number,
            comment);

        lock (_sync)
        {
            //TODO: Instead of logging to Console here, it should log to ILogger in separate class. ILogger should then log to console.
            Console.WriteLine($"Creating a new issue comment:\n{comment}");
        }
    }
}