using AiProgrammer.Solving.Model;

namespace AiProgrammer.Solving.GithubIssue;

public class InIssueNoChangesNotifier
{
    private readonly IGithubIssueContextProvider _contextProvider;

    public InIssueNoChangesNotifier(IGithubIssueContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }
    
    public async Task NotifyAboutNoChanges()
    {
        GithubIssueContext context = _contextProvider.Get();
        
        string comment = "Could not create Pull Request: no files changed. See action output for details";
        await context.GitHubClient.Issue.Comment.Create(context.RepoOwnerName, context.RepoName, context.SourceIssue.Number, comment);
    }
}
