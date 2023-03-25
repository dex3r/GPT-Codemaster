namespace AiProgrammer.Solving.GithubIssue;

public interface IIssueDescriptionProvider
{
    Task<string> GetIssueDescription();
}
