using System;
using AiProgrammer;
using AiProgrammer.Solving;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AiProgrammerTests;

public class DependenciesTests
{
    [Test]
    public void ConstructIGithubIssueSolver()
    {
        GithubIssueContext githubIssueContext = new GithubIssueContext(null, null, null, null);
        
        using IServiceScope serviceScope = AiProgrammerServices.CreateGithubIssueSolverServiceScope(githubIssueContext);

        IGithubIssueSolver issueSolver = serviceScope.ServiceProvider.GetRequiredService<IGithubIssueSolver>();

        Assert.NotNull(issueSolver);
    }
}
