using AiProgrammer;
using AiProgrammer.Github;
using AiProgrammer.Github.Issues;
using AiProgrammer.Github.Model;
using AiProgrammer.Solving;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Model;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace GithubIssueListener;

public class IssueSolvingStarter
{
    private readonly IInputProvider _inputProvider;
    private readonly IIssueUrlParser _issueUrlParser;
    private readonly IGithubClientProviderForToken _githubClientProvider;
    private readonly ISolverExceptionNotifier _solverExceptionNotifier;

    public IssueSolvingStarter(IInputProvider inputProvider, IIssueUrlParser issueUrlParser,
        IGithubClientProviderForToken githubClientProvider, ISolverExceptionNotifier solverExceptionNotifier)
    {
        _inputProvider = inputProvider;
        _issueUrlParser = issueUrlParser;
        _githubClientProvider = githubClientProvider;
        _solverExceptionNotifier = solverExceptionNotifier;
    }

    public async Task SendIssueToSolveInBrain()
    {
        IssueListenerInput input = _inputProvider.GetListenerInput();

        Console.WriteLine($"Selected issue found:");
        Console.WriteLine($"URL: {input.IssueUrl}");

        IGitHubClient githubClient = _githubClientProvider.Get(input.GithubToken);
        IssueInfo issueInfo = _issueUrlParser.ParseUrl(input.IssueUrl);
        RepoInfo repoInfo = issueInfo.Value.repoInfo;
        
        Issue issue = await githubClient.Issue.Get(repoInfo.Value.repoOwner,
            repoInfo.Value.repoName, issueInfo.Value.issueNumber);
        
        GithubIssueContext requestContext = new(githubClient, repoInfo.Value.repoOwner, 
            repoInfo.Value.repoName, issue);
        
        await Solve(requestContext);
    }

    private async Task Solve(GithubIssueContext requestContext)
    {
        try
        {
            using IServiceScope serviceScope = AiProgrammerServices.CreateGithubIssueSolverServiceScope(requestContext);

            IGithubIssueSolver issueSolver = serviceScope.ServiceProvider.GetRequiredService<IGithubIssueSolver>();
            
            await issueSolver.SolveGithubIssue();
        }
        catch (Exception e)
        {
            await _solverExceptionNotifier.TryToNotifyAboutException(requestContext, e);

            Console.WriteLine("Solving Issue failed. See exception below:");
            throw;
        }
    }
}
