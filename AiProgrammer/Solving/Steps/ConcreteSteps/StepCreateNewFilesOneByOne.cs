using System.Text;
using AiProgrammer.IO;
using AiProgrammer.Solving.Steps.Helpers;
using AiProgrammer.Solving.Utils;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepCreateNewFilesOneByOne : ISolverStep
{
    public bool ShowInStepsListForModel => true;
    public string? DescriptionForModel => "Create needed new files one by one";
    
    private const int MaxFilesToCreateLimit = 20;
    
    private readonly FollowNextStepExecutor _followNextStepExecutor;
    private readonly IProjectContentReader _projectContentReader;

    public StepCreateNewFilesOneByOne(FollowNextStepExecutor followNextStepExecutor, IProjectContentReader projectContentReader)
    {
        _followNextStepExecutor = followNextStepExecutor;
        _projectContentReader = projectContentReader;
    }
    
    public async Task<IReadOnlyCollection<FileContent>> GetFilesToCreate(List<string> alreadyReviewedFilesDescription)
    {
        List<FileContent> newFiles = new();
        List<string> newFilesDescription = new();

        string alreadyReviewedFilesFullDescription = BuildFullDescription(alreadyReviewedFilesDescription);
        
        for (int i = 0; i < MaxFilesToCreateLimit; i++)
        {
            bool created = await AskToCreateNewFile(alreadyReviewedFilesFullDescription, newFiles, newFilesDescription);

            if (!created)
            {
                return newFiles;
            }
        }

        throw new Exception($"Tried to create more files than the current limit of '{MaxFilesToCreateLimit}'");
    }

    private string BuildFullDescription(List<string> alreadyReviewedFilesDescription)
    {
        StringBuilder stringBuilder = new();

        foreach (string singleFile in alreadyReviewedFilesDescription)
        {
            stringBuilder.AppendLine(singleFile);
        }

        return stringBuilder.ToString();
    }

    private async Task<bool> AskToCreateNewFile(string alreadyReviewedFilesDescription, List<FileContent> newFiles,
        List<string> newFilesPathWithDescription)
    {
        StringBuilder promptBuilder = new();

        promptBuilder.AppendLine("Already reviewed files with your own description of what happened:");
        promptBuilder.AppendLine(alreadyReviewedFilesDescription);
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("New files you created:");
        foreach (string newFileDescription in newFilesPathWithDescription)
        {
            promptBuilder.AppendLine(newFileDescription);
        }

        promptBuilder.AppendLine();

        string doNotCreateMoreFilesCommand = "do_not_create_more_files";
        string crateFileCommand = "create_new_file";
        
        promptBuilder.AppendLine("Select one command with parameters to execute:");
        promptBuilder.AppendLine(
            $" - {doNotCreateMoreFilesCommand} - Use if there is no need to create a new file and move to the next step");
        promptBuilder.AppendLine(
            $" - {crateFileCommand} [path] [(in new line)new content in code block] [short description of what was created and why] - Create new file by" +
            " providing its content. Write a short description of what was created, this description will be read only by you when moving" +
            " to the next step");
        
        string prompt = promptBuilder.ToString();

        string stepResult = await _followNextStepExecutor.ExecuteStep(DescriptionForModel, prompt);
        stepResult = stepResult.ReplaceLineEndings("\n");

        if (stepResult.StartsWith(doNotCreateMoreFilesCommand))
        {
            return false;
        }

        if (stepResult.StartsWith(crateFileCommand))
        {
            ExecuteCreateFileCommand(stepResult, newFiles, newFilesPathWithDescription);
            return true;
        }

        Console.WriteLine($"Full output:\n{stepResult}");
        throw new UnexpectedAiOutputException($"AI Output is not a command.", stepResult);
    }

    private void ExecuteCreateFileCommand(string stepResult, List<FileContent> newFiles, List<string> newFilesPathWithDescription)
    {
        string[] parts = stepResult.Split("\n```\n");

        if (parts.Length != 3)
        {
            throw new UnexpectedAiOutputException("Invalid format for replace file command.", stepResult);
        }

        string path = parts[0];
        string fileContent = parts[1];
        string finalDescription = parts[2].ReplaceLineEndings(" ");

        if (_projectContentReader.DoesFileExist(FilePath.From(path)))
        {
            throw new Exception($"Tried to create file at path that already exist: '{path}'");
        }
        
        newFiles.Add(new FileContent(path, fileContent));
        newFilesPathWithDescription.Add($"{path} - created - {finalDescription}");
    }
}
