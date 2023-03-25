using AiProgrammer.Solving;
using AiProgrammer.Solving.Model;

namespace GithubIssueListener;

public class SolverExceptionNotifier : ISolverExceptionNotifier
{
    public async Task TryToNotifyAboutException(GithubIssueContext requestContext, Exception exception)
    {
        try
        {
            string message = $"Exception caught while GPT-Codemaster tried to solve the issue:\n" +
                             $"```\n" +
                             $"{exception}" +
                             $"```";

            await requestContext.GitHubClient.Issue.Comment.Create(requestContext.RepoOwnerName, requestContext.RepoName,
                requestContext.SourceIssue.Number, message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not notify about exception in the Issue comment. See exception below:");
            Console.WriteLine(e);
        }
    }
}
