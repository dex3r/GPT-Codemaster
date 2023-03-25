using Octokit;

namespace AiProgrammer.Github;

public interface IGithubContextClientProvider
{
    IGitHubClient Get();
}
