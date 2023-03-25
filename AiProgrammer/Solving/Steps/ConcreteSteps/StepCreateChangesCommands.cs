using AiProgrammer.IO;
using AiProgrammer.Solving.Commands;
using AiProgrammer.Solving.Steps.Helpers;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepCreateChangesCommands : ISolverStep
{
    public bool ShowInStepsListForModel => true;
    public string DescriptionForModel => "Create list of actions to perform on the files to program the user request";
    
    private const string CommandsSeparator = "#---";
    
    private readonly IProjectContentReader _projectContentReader;
    private readonly FollowNextStepExecutor _followNextStepExecutor;

    public StepCreateChangesCommands(IProjectContentReader projectContentReader, FollowNextStepExecutor followNextStepExecutor)
    {
        _projectContentReader = projectContentReader;
        _followNextStepExecutor = followNextStepExecutor;
    }
    
    public async Task<IReadOnlyList<string>> PerformStepCreateChangesCommands(IReadOnlyList<string> namesOfMethodsToPreview)
    {
        string classes = _projectContentReader.GetClassesWithSelectedMethodBodies(namesOfMethodsToPreview);

        string contentDescription = "Here is the content of those methods you wanted to see:";
        string content = $"\n" +
                         $"{classes}\n" +
                         $"\n";

        string stepDescription = "You are currently in step three (3). After seeing the methods contents create list of actions to" +
                                 " perform to program the user request. Available actions:\n" +
                                 "    - modify_method; [relative_file_path]; [namespace]; [class_name]; [method_name (with parameters)]; " +
                                 "- provide a new method body in new line including method signature\n" +
                                 "    - add_method; [relative_file_path]; [namespace]; [class_name]; [method_name]; " +
                                 "- provide a new full method in new line including method signature\n" +
                                 "    - add_file; [relative_file_path]; " +
                                 "- create a completely new file with class, interface, struct etc. with a source code below." +
                                 "\n" +
                                 "Only output commands and code blocks. Everything you want to explain do in comments, however," +
                                 " keep comments to be very concise and put them only if it’s not obvious WHY something is done," +
                                 $" not what. Between every command put a new line with \"{CommandsSeparator}\"" +
                                 " Make sure to put actual code in code blocks using ```. Always print " +
                                 "the whole source, never the 'The rest of the class remains unchanged' comment.";

        string rawResult = await _followNextStepExecutor.ExecuteStep(contentDescription, content, stepDescription);
        
        if (string.IsNullOrEmpty(rawResult))
        {
            return Array.Empty<string>();
        }

        rawResult = rawResult.ReplaceLineEndings("\n");

        string[] commands = rawResult.Split($"\n{CommandsSeparator}\n");
        return commands;
    }
}
