using AiProgrammer.IO;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Steps.ConcreteSteps;
using Octokit;

/* Original idea for this solver:
 
 # Steps:
1. Select files to review
2. Modify existing files
3. Create new files

# Step: Modify files

Already revied files with your own description of what happened:
A/B/C.cs - modified - [what AI said it modified]
A/D/F.cs - not modified

Files left to review:
A/X.cs
A/Y.cs

Current file:
A/M.cs

Content:
```
[File content]
```

Select one command with parameters to execute:
 - next_file - Do not modify this file and go to the nex
 - replace_file (new content in code block) (description of what was modified and why) - Modify or remove the file by re-writing it's content. Write a short description of what was modified, this description will be read only by you when moving to the next file
 
# Step: Create new files

Already reviewed files with your own description of what happened:
A/B/C.cs - modified - [what AI said it modified]
A/D/F.cs - not modified

New files you created:
none

Select one command with parameters to execute:
 - create_file (path) (new content in code block) (description of what was created and why) - Create new file by providing its content. Write a short description of what was created, this description will be read only by you when moving to the next step
 - do_not_create_more_files - Use if there is no need to create a new file and move to the next step

 */

namespace AiProgrammer.Solving.Steps.EntrySteps;

public class StepSolveGithubIssueFileByFile: ISolverStepWithSubsteps
{
    public bool ShowInStepsListForModel => false;
    public string? DescriptionForModel => null;
    
    public IReadOnlyList<ISolverStep> Steps { get; }

    private readonly StepSelectFilesToPreview _stepSelectFiles;
    private readonly StepModifyExistingFilesOneByOne _stepModifyExistingFilesOneByOne;
    private readonly StepCreateNewFilesOneByOne _stepCreateNewFilesOneByOne;
    private readonly StepCreatePullRequest _stepCreatePullRequest;
    private readonly StepNotifyAboutPullRequest _notifyAboutPullRequest;
    private readonly InIssueNoChangesNotifier _noChangesNotifier;

    public StepSolveGithubIssueFileByFile(
        StepSelectFilesToPreview stepSelectFiles,
        StepModifyExistingFilesOneByOne stepModifyExistingFilesOneByOne,
        StepCreateNewFilesOneByOne stepCreateNewFilesOneByOne,
        InIssueNoChangesNotifier noChangesNotifier,
        StepCreatePullRequest stepCreatePullRequest,
        StepNotifyAboutPullRequest notifyAboutPullRequest)
    {
        _stepSelectFiles = stepSelectFiles;
        _stepModifyExistingFilesOneByOne = stepModifyExistingFilesOneByOne;
        _stepCreateNewFilesOneByOne = stepCreateNewFilesOneByOne;
        _noChangesNotifier = noChangesNotifier;
        _stepCreatePullRequest = stepCreatePullRequest;
        _notifyAboutPullRequest = notifyAboutPullRequest;

        Steps = new ISolverStep[]
        {
            _stepSelectFiles,
            _stepModifyExistingFilesOneByOne,
            _stepCreateNewFilesOneByOne,
            _stepCreatePullRequest,
            _notifyAboutPullRequest,
        };
    }

    public async Task SolveGithubIssue()
    {
        IReadOnlyList<FilePath> filesToPreview = await _stepSelectFiles.GetFilesToPreviewOrdered();
        StepModifyExistingFilesOneByOne.Result changedExistingFiles = await _stepModifyExistingFilesOneByOne.GetChangedFiles(filesToPreview);
        
        IReadOnlyCollection<FileContent> newFiles = await _stepCreateNewFilesOneByOne
            .GetFilesToCreate(changedExistingFiles.PotentiallyModifiedFilesWithDescription);

        IReadOnlyList<FileContent> allChanges = changedExistingFiles.ModifiedFilesContent.Concat(newFiles).ToArray();

        if (allChanges.Count == 0)
        {
            await _noChangesNotifier.NotifyAboutNoChanges();
            return;
        }
        
        PullRequest pullRequest = await _stepCreatePullRequest.CreatePullRequest(allChanges);
        await _notifyAboutPullRequest.NotifyAboutNewPullRequest(pullRequest);
    }
}
