using AiProgrammer.IO;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Steps.ConcreteSteps;
using Octokit;

namespace AiProgrammer.Solving.Steps.EntrySteps;

public class StepSolveGithubIssueWithMethodSelection : ISolverStepWithSubsteps
{
    public bool ShowInStepsListForModel => false;
    public string? DescriptionForModel => null;
    
    public IReadOnlyList<ISolverStep> Steps { get; }
    
    private readonly StepSelectFilesToPreview _stepSelectFiles;
    private readonly StepSelectMethodsToPreview _stepSelectMethods;
    private readonly StepCreateChangesCommands _createChangesCommands;
    private readonly StepChangeFilesUsingCommands _changeFilesUsingCommands;
    private readonly StepCreatePullRequest _stepCreatePullRequest;
    private readonly StepNotifyAboutPullRequest _notifyAboutPullRequest;
    private readonly InIssueNoChangesNotifier _noChangesNotifier;

    public StepSolveGithubIssueWithMethodSelection(
        StepSelectFilesToPreview stepSelectFiles,
        StepSelectMethodsToPreview stepSelectMethods,
        StepCreateChangesCommands createChangesCommands,
        StepChangeFilesUsingCommands changeFilesUsingCommands,
        StepCreatePullRequest stepCreatePullRequest,
        StepNotifyAboutPullRequest notifyAboutPullRequest,
        InIssueNoChangesNotifier noChangesNotifier)
    {
        _stepSelectFiles = stepSelectFiles;
        _stepSelectMethods = stepSelectMethods;
        _createChangesCommands = createChangesCommands;
        _changeFilesUsingCommands = changeFilesUsingCommands;
        _stepCreatePullRequest = stepCreatePullRequest;
        _notifyAboutPullRequest = notifyAboutPullRequest;
        _noChangesNotifier = noChangesNotifier;

        Steps = new ISolverStep[]
        {
            _stepSelectFiles,
            _stepSelectMethods,
            _createChangesCommands,
            _changeFilesUsingCommands,
            _stepCreatePullRequest,
            _notifyAboutPullRequest
        };
    }

    public async Task SolveGithubIssue()
    {
        IReadOnlyList<FilePath> filesToPreview = await _stepSelectFiles.GetFilesToPreviewOrdered();
        IReadOnlyList<string> methodsToPreview = await _stepSelectMethods.GetMethodsToPreviewOrdered(filesToPreview);
        IReadOnlyList<string> commands = await _createChangesCommands.PerformStepCreateChangesCommands(methodsToPreview);
        IReadOnlyCollection<FileContent> changes = await _changeFilesUsingCommands.GetChangedFiles(commands);
        
        if (changes.Count == 0)
        {
            await _noChangesNotifier.NotifyAboutNoChanges();
            return;
        }
        
        PullRequest pullRequest = await _stepCreatePullRequest.CreatePullRequest(changes);
        await _notifyAboutPullRequest.NotifyAboutNewPullRequest(pullRequest);
    }
}
