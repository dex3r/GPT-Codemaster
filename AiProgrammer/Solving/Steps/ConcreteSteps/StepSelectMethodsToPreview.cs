using AiProgrammer.IO;
using AiProgrammer.Solving.Model;
using AiProgrammer.Solving.Steps.Helpers;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepSelectMethodsToPreview : ISolverStep
{
    public bool ShowInStepsListForModel => true;
    public string DescriptionForModel => 
        "From a list of files select methods that you want to see. You have a limited memory so you will only see the" + 
        " selected ones and you will be only able to modify those.";
    
    private readonly IProjectContentReader _projectContentReader;
    private readonly FollowNextStepExecutor _followNextStepExecutor;

    public StepSelectMethodsToPreview(IProjectContentReader projectContentReader, FollowNextStepExecutor followNextStepExecutor)
    {
        _projectContentReader = projectContentReader;
        _followNextStepExecutor = followNextStepExecutor;
    }
    
    public async Task<IReadOnlyList<string>> GetMethodsToPreviewOrdered(IReadOnlyList<FilePath> filesToPreview)
    {
        string classes = _projectContentReader.GetClassesWithoutMethodBodies(filesToPreview);

        string contentDescription = "Here is the content of those classes without method bodies:";
        string content = $"\n" +
                         $"{classes}\n" +
                         $"\n";

        string stepDescription =
            "You are currently in step two (2). Please provide a list of methods from those classes you think will help you the most " +
            "when solving the problem. List them from most likely to least likely, since you will only see those that will fit in your " +
            "memory. Print them in this format:\n" +
            "[file_path] [method_name]";

        string rawResult = await _followNextStepExecutor.ExecuteStep(contentDescription, content, stepDescription);

        if (string.IsNullOrEmpty(rawResult))
        {
            return ArraySegment<string>.Empty;
        }

        string[] methods = rawResult.Split("\n");
        return methods;
    }
}
