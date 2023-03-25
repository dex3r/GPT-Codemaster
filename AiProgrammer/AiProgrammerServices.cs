using AiProgrammer.AiInterface;
using AiProgrammer.CodeProcessing;
using AiProgrammer.Github;
using AiProgrammer.Github.PullRequests;
using AiProgrammer.IO;
using AiProgrammer.Logging;
using AiProgrammer.Solving.GithubIssue;
using AiProgrammer.Solving.Model;
using AiProgrammer.Solving.Steps;
using AiProgrammer.Solving.Steps.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AiProgrammer
{
    [PublicAPI]
    public static class AiProgrammerServices
    {
        public static IServiceScope CreateGithubIssueSolverServiceScope(GithubIssueContext githubIssueContext)
        {
            IServiceProvider basicServices = GetServices();
            return CreateGithubIssueSolverServiceScope(githubIssueContext, basicServices);
        }
        
        public static IServiceScope CreateGithubIssueSolverServiceScope(GithubIssueContext githubIssueContext,
            IServiceProvider basicServices)
        {
            IServiceScopeFactory serviceScopeFactory = basicServices.GetRequiredService<IServiceScopeFactory>();
            
            IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            
            serviceScope.ServiceProvider.GetRequiredService<GithubIssueContextProvider>().Set(githubIssueContext);

            return serviceScope;
        }
        
        public static IServiceProvider GetServices()
        {
            IServiceCollection services = new ServiceCollection();
            AddBasicSingletons(services);
            AddUniversalScopedServices(services);
            AddGithubIssueScopedServices(services);

            return services.BuildServiceProvider();
        }
        
        private static void AddBasicSingletons(IServiceCollection services)
        {
            services.AddSingleton<IClassExtractor, ClassExtractorCSharp>();
            services.AddSingleton<PullRequestCreator>();
        }

        private static void AddUniversalScopedServices(IServiceCollection services)
        {
            services.AddScoped<IProjectContentReader, ProjectContentReader>();
            services.AddScoped<IChat, ChatCompletions>();
            services.AddScoped<FollowNextStepExecutor>();
            services.AddScoped<ICompletions, ChatCompletions>();
            services.AddScoped<IIssueDescriptionProvider, IssueDescriptionProvider>();
            services.AddScoped<ICurrentStepsHolder, CurrentStepsHolder>();

            AddAllSolverSteps(services);
        }

        private static void AddAllSolverSteps(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<ISolverStep>()
                .AddClasses(classes => classes.AssignableTo<ISolverStep>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        }

        private static void AddGithubIssueScopedServices(IServiceCollection services)
        {
            services.AddScoped<GithubIssueContextProvider>();
            services.AddScoped<IGithubIssueContextProvider>(x => x.GetRequiredService<GithubIssueContextProvider>());

            services.AddScoped<IGithubContextClientProvider, GithubContextClientProvider>();
            services.AddScoped<ISolverProgressLogger, SolverProgressLoggerInIssue>();
            services.AddScoped<InIssueNoChangesNotifier>();
            
            services.AddScoped<IGithubIssueSolver, GithubIssueSolverFileByFile>();
        }
    }
}
