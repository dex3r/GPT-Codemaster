using AiProgrammer.Github.Model;

namespace AiProgrammer.Github.Issues;

public class IssueUrlParser : IIssueUrlParser
{
    public IssueInfo ParseUrl(string url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }
        
        // Expected URL format: https://github.com/{owner}/{repo}/issues/{issueNumber}
        string[] segments = new Uri(url).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length >= 4 && segments[^2] == "issues" && int.TryParse(segments[^1], out int issueNumber))
        {
            RepoInfo repoInfo = RepoInfo.From((RepoOwner.From(segments[^4]),
                RepoName.From(segments[^3])));

            return IssueInfo.From((repoInfo, issueNumber));
        }

        throw new ArgumentException($"Given URL is not a valid Github Issue URL: '{url}'", nameof(url));
    }
}
