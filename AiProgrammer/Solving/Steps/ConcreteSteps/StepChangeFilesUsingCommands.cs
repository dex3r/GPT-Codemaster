using AiProgrammer.AiInterface;
using AiProgrammer.IO;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepChangeFilesUsingCommands : ISolverStep
{
    public bool ShowInStepsListForModel => false;
    public string? DescriptionForModel => null;
    
    private readonly ICompletions _completions;

    public StepChangeFilesUsingCommands(ICompletions completions)
    {
        _completions = completions;
    }
    
    public async Task<IReadOnlyCollection<FileContent>> GetChangedFiles(IReadOnlyList<string> commands)
    {
        FileContent?[] changes = await ApplyChangesSequentially(commands);

        return changes.Where(x => x != null).ToArray()!;
    }
    
    private async Task<FileContent?[]> ApplyChangesSequentially(IReadOnlyList<string> commands)
    {
        FileContent?[] changes = new FileContent[commands.Count];

        for (int i = 0; i < commands.Count; i++)
        {
            changes[i] = await TryGetChangeFromCommand(commands[i]);
        }

        return changes;
    }
    
    private async Task<FileContent?> TryGetChangeFromCommand(string commandFullString)
    {
        string[] commandLines = commandFullString.Split('\n');

        string actualCommand = commandLines[0];

        if (string.IsNullOrEmpty(actualCommand) && commandLines.Length > 1)
        {
            actualCommand = commandLines[1];
        }

        string[] commandParameters = actualCommand.Split(';');

        if (commandParameters.Length < 2)
        {
            Console.WriteLine($"Provided command is in invalid format. Expected at least two parameters. Command: '{actualCommand}'");
            return null;
        }

        string fileRelativePath = commandParameters[1].Trim();

        string? currentFileContent = await ProjectContentReader.GetFileContent(fileRelativePath);

        string chatMessage = $"Given this source code taken from location '{fileRelativePath}':\n" +
                             $"```\n" +
                             $"{currentFileContent}\n" +
                             $"```\n" +
                             $"\n" +
                             $"Execute the following command modifying above source code and print the entire code with modifications. " +
                             $"Do not put the code in a code block. Do not add anything else. Always print the full result.\n" +
                             $"\n" +
                             $"{commandFullString}";

        string systemMessage =
            "Your task is to execute given command and modify given source code. When creating new classes, structs etc. the source code " +
            "might be empty. Always print the full source code, never replace it with a 'The rest of the class remains unchanged' comment.";

        string response = await _completions.GetCompletion(systemMessage, chatMessage);

        return new FileContent(fileRelativePath, response);
    }
}
