using AiProgrammer.Solving.Steps.EntrySteps;
using AiProgrammer.Solving.Steps.Helpers;

namespace AiProgrammer.Solving.GithubIssue;

public record GithubIssueSolverWithMethodSelection(
    StepSolveGithubIssueWithMethodSelection SolveStep, 
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
