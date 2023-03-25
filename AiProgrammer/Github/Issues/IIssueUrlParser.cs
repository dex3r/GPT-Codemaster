using AiProgrammer.Github.Model;

namespace AiProgrammer.Github.Issues;

public interface IIssueUrlParser
{
    IssueInfo ParseUrl(string url);
}
