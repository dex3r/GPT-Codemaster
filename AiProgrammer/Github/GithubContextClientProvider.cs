using AiProgrammer.Solving;
using AiProgrammer.Solving.GithubIssue;
using Octokit;

namespace AiProgrammer.Github;

public class GithubContextClientProvider : IGithubContextClientProvider
{
    private readonly IGithubIssueContextProvider _githubIssueContextProvider;

    public GithubContextClientProvider(IGithubIssueContextProvider githubIssueContextProvider)
    {
        _githubIssueContextProvider = githubIssueContextProvider;
    }
    
    public IGitHubClient Get() => _githubIssueContextProvider.Get().GitHubClient;
}