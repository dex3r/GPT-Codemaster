using AiProgrammer.Solving;
using AiProgrammer.Solving.Model;

namespace GithubIssueListener;

public interface ISolverExceptionNotifier
{
    Task TryToNotifyAboutException(GithubIssueContext requestContext, Exception exception);
}
