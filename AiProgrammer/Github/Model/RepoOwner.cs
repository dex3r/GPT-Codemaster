using ValueOf;

namespace AiProgrammer.Github.Model;

public class RepoOwner : ValueOf<string, RepoOwner>
{
    public static implicit operator string(RepoOwner value) => value.Value;
}
