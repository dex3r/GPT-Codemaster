using AiProgrammer;
using AiProgrammer.Github;
using AiProgrammer.Github.Issues;
using AiProgrammer.Solving;

namespace GithubIssueListener
{
    public static class ListenerEntryPoint
    {
        public static async Task Main()
        {
            Console.WriteLine("GPT-Codemaster starting");
            
            IssueSolvingStarter issueSolvingStarter = new(
                new InputProviderFromEnvironmentVariables(),
                new IssueUrlParser(),
                new GithubClientProviderForToken(),
                new SolverExceptionNotifier());
            
            try
            {
                await issueSolvingStarter.SendIssueToSolveInBrain();
                Console.WriteLine("GPT-Codemaster finished successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
