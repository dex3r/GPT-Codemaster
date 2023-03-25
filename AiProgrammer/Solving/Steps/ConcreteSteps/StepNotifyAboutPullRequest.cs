using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Model;
using Octokit;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepNotifyAboutPullRequest : ISolverStep
{
    public bool ShowInStepsListForModel => false;
    public string? DescriptionForModel => null;
    
    private readonly IGithubIssueContextProvider _contextProvider;
    
    public StepNotifyAboutPullRequest(IGithubIssueContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }
    
    public async Task NotifyAboutNewPullRequest(PullRequest pullRequest)
    {
        GithubIssueContext context = _contextProvider.Get();
        
        int pullRequestNumber = pullRequest.Number;
        string pullRequestLink = $"https://github.com/{context.RepoOwnerName}/{context.RepoName}/pull/{pullRequestNumber}";

        string comment = $"Pull Request created: {pullRequestLink}";
        await context.GitHubClient.Issue.Comment.Create(context.RepoOwnerName, context.RepoName, context.SourceIssue.Number, comment);
    }
}
