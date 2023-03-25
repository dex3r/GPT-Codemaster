using Octokit;

namespace AiProgrammer.Github;

public interface IGithubClientProviderForToken
{
    IGitHubClient Get(string githubToken);
}
