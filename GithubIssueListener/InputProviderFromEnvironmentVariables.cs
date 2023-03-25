namespace GithubIssueListener;

public class InputProviderFromEnvironmentVariables : IInputProvider
{
    public IssueListenerInput GetListenerInput()
    {
        const string issueUrlEnvKey = "ISSUE_URL";
        const string githubTokenEvnKey = "GITHUB_TOKEN";
        
        string? issueUrl = Environment.GetEnvironmentVariable(issueUrlEnvKey);

        if (string.IsNullOrEmpty(issueUrl))
        {
            throw new Exception($"Missing required environment variable: {issueUrlEnvKey}");
        }
        
        string? githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(githubToken))
        {
            throw new Exception($"Missing required environment variable: {githubTokenEvnKey}");
        }

        return new IssueListenerInput(issueUrl, githubToken);
    }
}
