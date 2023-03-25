using AiProgrammer.Solving.Steps.EntrySteps;
using AiProgrammer.Solving.Steps.Helpers;

namespace AiProgrammer.Solving.GithubIssue;

public record GithubIssueSolverFileByFile(
    StepSolveGithubIssueFileByFile SolveStep, 
    ICurrentStepsHolder CurrentStepsHolder
    ) : IGithubIssueSolver
{
    public async Task SolveGithubIssue()
    {
        try
        {
            CurrentStepsHolder.SetCurrentSteps(SolveStep);
            
            await SolveStep.SolveGithubIssue();
        }
        finally
        {
            CurrentStepsHolder.ClearCurrentSteps();
        }
    }
}
