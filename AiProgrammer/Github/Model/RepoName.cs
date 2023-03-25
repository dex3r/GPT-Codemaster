using ValueOf;

namespace AiProgrammer.Github.Model;

public class RepoName : ValueOf<string, RepoName>
{
    public static implicit operator string(RepoName value) => value.Value;
}
