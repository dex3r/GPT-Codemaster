using AiProgrammer.IO;
using AiProgrammer.Solving;
using AiProgrammer.Solving.Model;
using Octokit;
using Octokit.Helpers;

namespace AiProgrammer.Github.PullRequests;

public class PullRequestCreator
{
    public async Task<PullRequest> CreatePullRequest(GithubIssueContext context, IReadOnlyCollection<FileContent> changedFiles)
    {
        //TODO: Let users select source branch from issue. Potentially try to ask GPT-4 if there is a branch name stated in the issue desc.
        string sourceBranch = "main";

        string newBranchName = await GetNewBranchName(context);
        await context.GitHubClient.Git.Reference.CreateBranch(context.RepoOwnerName, context.RepoName,
            newBranchName, sourceBranch);

        Reference newReference = await CreateCommit(context, newBranchName, changedFiles);
        
        NewPullRequest newPullRequest = new NewPullRequest($"[GPT-Codemaster] Fixes issue #{context.SourceIssue.Number}: {context.SourceIssue.Title}",
            newReference.Ref, sourceBranch);

        newPullRequest.Body = "This Pull Request was created automatically by GPT-Codemaster\n" +
                              $"Resolves #{context.SourceIssue.Number}";

        PullRequest pullRequest = await context.GitHubClient.PullRequest.Create(context.RepoOwnerName, context.RepoName, newPullRequest);

        return pullRequest;
    }

    private static async Task<Reference> CreateCommit(GithubIssueContext context, string sourceBranch,
        IReadOnlyCollection<FileContent> changedFiles)
    {
        string headMasterRef = $"heads/{sourceBranch}";
        Reference? masterReference = await context.GitHubClient.Git.Reference.Get(context.RepoOwnerName, context.RepoName,
            headMasterRef); // Get reference of master branch

        Commit? latestCommit = await context.GitHubClient.Git.Commit.Get(context.RepoOwnerName, context.RepoName,
            masterReference.Object.Sha); // Get the latest commit of this branch

        NewTree newTree = new NewTree { BaseTree = latestCommit.Tree.Sha };

        IEnumerable<NewTreeItem> treeItems = GetTreeItems(changedFiles);

        foreach (NewTreeItem newTreeItem in treeItems)
        {
            newTree.Tree.Add(newTreeItem);
        }

        TreeResponse? treeResponse = await context.GitHubClient.Git.Tree.Create(context.RepoOwnerName, context.RepoName,
            newTree);

        NewCommit newCommit = new($"[GPT-Codemaster] Progress on issue #{context.SourceIssue.Number}: {context.SourceIssue.Title}",
            treeResponse.Sha, masterReference.Object.Sha);

        Commit? commit = await context.GitHubClient.Git.Commit.Create(context.RepoOwnerName, context.RepoName, newCommit);

        Reference newReference = await context.GitHubClient.Git.Reference.Update(context.RepoOwnerName, context.RepoName,
            headMasterRef, new ReferenceUpdate(commit.Sha));

        return newReference;
    }

    private static IEnumerable<NewTreeItem> GetTreeItems(IReadOnlyCollection<FileContent> changedOrNewFiles)
    {
        return changedOrNewFiles.Select(changedFile => new NewTreeItem
        {
            Mode = "100644",
            Path = changedFile.FilePath.Replace('\\', '/'), // Git always expects to have files separated by forward slash
            Type = TreeType.Blob,
            Content = changedFile.Content
        });
    }

    private static async Task<string> GetNewBranchName(GithubIssueContext context)
    {
        IReadOnlyList<Branch> branches = await context.GitHubClient.Repository.Branch.GetAll(context.RepoOwnerName,
            context.RepoName)!;

        string newBranchNameBase = $"gpt-codemaster/issue/{context.SourceIssue.Number}";

        const int branchesLimit = 100;

        for (int i = 1; i < branchesLimit; i++)
        {
            string finalBranchName = $"{newBranchNameBase}/{i}";

            if (branches.Any(existingBranch => existingBranch.Name == finalBranchName))
            {
                continue;
            }

            return finalBranchName;
        }

        throw new Exception($"All branches names has been used up! Cannot create more branches. Limit: {branchesLimit}");
    }
}
