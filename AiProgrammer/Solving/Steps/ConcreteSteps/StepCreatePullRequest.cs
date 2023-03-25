using AiProgrammer.Github.PullRequests;
using AiProgrammer.IO;
using AiProgrammer.Solving.GithubIssue;
using Octokit;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepCreatePullRequest : ISolverStep
{
    public bool ShowInStepsListForModel => false;
    public string? DescriptionForModel => null;
    
    private readonly PullRequestCreator _pullRequestCreator;
    private readonly IGithubIssueContextProvider _contextProvider;

    public StepCreatePullRequest(PullRequestCreator pullRequestCreator, IGithubIssueContextProvider contextProvider)
    {
        _pullRequestCreator = pullRequestCreator;
        _contextProvider = contextProvider;
    }
    
    public async Task<PullRequest> CreatePullRequest(IReadOnlyCollection<FileContent> changedFiles)
    {
        return await _pullRequestCreator.CreatePullRequest(_contextProvider.Get(), changedFiles);
    }
}
