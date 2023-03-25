using AiProgrammer.Github.Issues;
using Microsoft.Extensions.DependencyInjection;

namespace AiProgrammer.Github;

public static class GithubServices
{
    public static void AddGithubServices(this IServiceCollection services)
    {
        services.AddSingleton<IGithubClientProviderForToken, GithubClientProviderForToken>();
        services.AddSingleton<IIssueUrlParser, IssueUrlParser>();
    }
}
