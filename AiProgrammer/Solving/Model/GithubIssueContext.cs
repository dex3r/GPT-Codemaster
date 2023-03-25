using AiProgrammer.Github.Model;
using Octokit;

namespace AiProgrammer.Solving.Model;

public record GithubIssueContext(IGitHubClient GitHubClient, RepoOwner RepoOwnerName,
    RepoName RepoName, Issue SourceIssue)
{
    public string GetIssueDescription() => SourceIssue.Title + "\n" + SourceIssue.Body;
}
