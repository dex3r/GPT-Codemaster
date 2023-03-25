namespace AiProgrammer.Solving.GithubIssue;

public class IssueDescriptionProvider : IIssueDescriptionProvider
{
    private readonly IGithubIssueContextProvider _githubIssueContextProvider;

    public IssueDescriptionProvider(IGithubIssueContextProvider githubIssueContextProvider)
    {
        _githubIssueContextProvider = githubIssueContextProvider;
    }

    public Task<string> GetIssueDescription() => Task.FromResult(_githubIssueContextProvider.Get().GetIssueDescription());
}