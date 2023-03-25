using System.Text;
using AiProgrammer.IO;
using AiProgrammer.Solving.Steps.Helpers;
using AiProgrammer.Solving.Utils;

namespace AiProgrammer.Solving.Steps.ConcreteSteps;

public class StepModifyExistingFilesOneByOne : ISolverStep
{
    public record Result(IReadOnlyList<FileContent> ModifiedFilesContent, List<string> PotentiallyModifiedFilesWithDescription);
    
    public bool ShowInStepsListForModel => true;
    public string DescriptionForModel => "Modify selected existing files one by one";
    
    private readonly FollowNextStepExecutor _followNextStepExecutor;
    private readonly IProjectContentReader _projectContentReader;

    public StepModifyExistingFilesOneByOne(FollowNextStepExecutor followNextStepExecutor, IProjectContentReader projectContentReader)
    {
        _followNextStepExecutor = followNextStepExecutor;
        _projectContentReader = projectContentReader;
    }

    public async Task<Result> GetChangedFiles(IReadOnlyList<FilePath> filesToPreview)
    {
        List<string> alreadyReviewedFilesTexts = new();
        Queue<FilePath> filesLeftToReview = new(filesToPreview);
        List<FileContent> modifiedFiles = new();

        while (filesLeftToReview.Count > 0)
        {
            FilePath currentFile = filesLeftToReview.Dequeue();
            FileContent? modifiedFile = await ExecuteForSingleFile(currentFile, alreadyReviewedFilesTexts, filesLeftToReview);

            if (modifiedFile != null)
            {
                modifiedFiles.Add(modifiedFile);
            }
        }

        return new Result(modifiedFiles, alreadyReviewedFilesTexts);
    }

    private async Task<FileContent?> ExecuteForSingleFile(FilePath currentFile, List<string> alreadyReviewedFilesTexts,
       IReadOnlyCollection<FilePath> filesLeftToReview)
    {
        StringBuilder promptBuilder = new();

        promptBuilder.AppendLine("Already reviewed files with your own description of what happened:");
        foreach (string alreadyReviewedFile in alreadyReviewedFilesTexts)
        {
            promptBuilder.AppendLine(alreadyReviewedFile);
        }

        promptBuilder.AppendLine();
        
        promptBuilder.AppendLine("Files left to review:");
        foreach (FilePath fileLeftToReview in filesLeftToReview)
        {
            promptBuilder.AppendLine(fileLeftToReview.Value);
        }

        promptBuilder.AppendLine();

        promptBuilder.AppendLine("Current file path:");
        promptBuilder.AppendLine(currentFile.Value);
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("Current file content:");
        promptBuilder.AppendLine("```");
        promptBuilder.AppendLine(_projectContentReader.ReadFile(currentFile));
        promptBuilder.AppendLine("```");

        string nextFileCommand = "next_file";
        string replaceFileCommand = "replace_file";

        promptBuilder.AppendLine("Select one command with this parameters to execute:");
        
        promptBuilder.AppendLine(@$"
 - {nextFileCommand} [Note to yourself of what you saw that can be useful in next steps to solve the issue
 and when changing or creating next files. Make sure to include all needed information as you won't see this file again and won't remember
 what was inside.] - Do not modify this file and go to the next");
        
        promptBuilder.AppendLine(@$"
 - {replaceFileCommand} [(in new line) new content in code block] [short description of what was actually modified and
 why, that can be useful in next steps] - Modify or remove the file by re-writing it's content. Write a short description of what was
 modified, this description will be read only by you when moving to the next file");

        string prompt = promptBuilder.ToString();

        string stepResult = await _followNextStepExecutor.ExecuteStep(DescriptionForModel, prompt);
        stepResult = stepResult.ReplaceLineEndings("\n");

        if (stepResult.StartsWith(nextFileCommand))
        {
            ExecuteNextFileCommand(nextFileCommand, currentFile, alreadyReviewedFilesTexts, stepResult);
            return null;
        }

        if (stepResult.StartsWith(replaceFileCommand))
        {
            return ExecuteReplaceFileCommand(currentFile, alreadyReviewedFilesTexts, stepResult);
        }

        Console.WriteLine($"Full output:\n{stepResult}");
        throw new UnexpectedAiOutputException($"AI Output is not a command.", stepResult);
    }

    private void ExecuteNextFileCommand(string nextFileCommand, FilePath currentFile, List<string> alreadyReviewedFilesTexts,
        string stepResult)
    {
        if (stepResult.StartsWith(nextFileCommand) == false)
        {
            throw new UnexpectedAiOutputException("Invalid format for next file command.", stepResult);
        }

        string note = stepResult.Substring(nextFileCommand.Length);

        alreadyReviewedFilesTexts.Add($"{currentFile.Value} - not modified - {note}");
    }

    private FileContent ExecuteReplaceFileCommand(FilePath currentFile, List<string> alreadyReviewedFilesTexts,
        string stepResult)
    {
        string[] parts = stepResult.Split("\n```\n");

        if (parts.Length != 3)
        {
            throw new UnexpectedAiOutputException("Invalid format for replace file command.", stepResult);
        }
        
        string fileContent = parts[1];
        string finalDescription = parts[2].ReplaceLineEndings(" ");
        
        alreadyReviewedFilesTexts.Add($"{currentFile.Value} - modified - {finalDescription}");

        return new FileContent(currentFile.Value, fileContent);
    }
}
