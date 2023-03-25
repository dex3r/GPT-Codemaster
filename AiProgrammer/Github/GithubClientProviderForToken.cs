using Octokit;

namespace AiProgrammer.Github;

public class GithubClientProviderForToken : IGithubClientProviderForToken
{
    public IGitHubClient Get(string githubToken)
    {
        return new GitHubClient(new ProductHeaderValue(CodeStatics.ProjectName))
        {
            Credentials = new Credentials(githubToken)
        };
    }
}
