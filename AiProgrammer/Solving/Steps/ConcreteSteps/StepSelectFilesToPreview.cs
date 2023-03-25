using AiProgrammer.IO;
using AiProgrammer.Solving.Steps.Helpers;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepSelectFilesToPreview : ISolverStep
{
    public bool ShowInStepsListForModel => true;
    public string DescriptionForModel => 
        "From a list of files select ones that will be needed to see or modify to program the user request." + 
        " You will be shown content of those files. You have a limited memory so you will only see the selected ones.";
    
    private readonly FollowNextStepExecutor _followNextStepExecutor;

    public StepSelectFilesToPreview(FollowNextStepExecutor followNextStepExecutor)
    {
        _followNextStepExecutor = followNextStepExecutor;
    }
    
    public async Task<IReadOnlyList<FilePath>> GetFilesToPreviewOrdered()
    {
        string contentDescription = "Here is a hierarchical view of files in the project:";
        string content = ProjectContentReader.GetFilesPaths().Aggregate("", (accumulated, next) => $"{accumulated}\n{next}");
        string stepDescription =
            "You are currently in step one (1). Please provide a list of files you think will help you the most when solving the " +
            "problem. List them from most likely to help solve to problem to least likely. " +
            "Print each path in new line without numerating them. Make sure to always print a full path.";

        string rawResult = await _followNextStepExecutor.ExecuteStep(contentDescription, content, stepDescription);

        if (string.IsNullOrEmpty(rawResult))
        {
            return ArraySegment<FilePath>.Empty;
        }

        string[] paths = rawResult.Split("\n");
        
        return paths.Select(FilePath.From).ToArray();
    }
}
