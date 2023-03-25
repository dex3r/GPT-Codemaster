using System.Text;
using AiProgrammer.AiInterface;
using AiProgrammer.Solving.GithubIssue;

namespace AiProgrammer.Solving.Steps.Helpers;

public class FollowNextStepExecutor
{
    private readonly ICompletions _completions;
    private readonly IIssueDescriptionProvider _issueDescriptionProvider;
    private readonly ICurrentStepsHolder _currentStepsHolder;
    private string _customSystemMessage;

    public FollowNextStepExecutor(ICompletions completions, IIssueDescriptionProvider issueDescriptionProvider,
        ICurrentStepsHolder currentStepsHolder)
    {
        _completions = completions;
        _issueDescriptionProvider = issueDescriptionProvider;
        _currentStepsHolder = currentStepsHolder;
        _customSystemMessage = null;
    }
    
    public void SetCustomSystemMessage(string customSystemMessage)
    {
        _customSystemMessage = customSystemMessage;
    }

    public async Task<string> ExecuteStep(string contentDescription, string content, string currentStepDescription)
    {
        string stepContent = $"{contentDescription}:\n" +
                             $"\n" +
                             $"{content}\n" +
                             $"\n" +
                             $"---" +
                             $"\n" +
                             $"{currentStepDescription}";

        string issueDescription = await _issueDescriptionProvider.GetIssueDescription();

        string systemMessage = GetSystemMessage();
        string userMessage = GetFullUserInput(issueDescription, stepContent);

        return await _completions.GetCompletion(systemMessage, userMessage);
    }
    
    public async Task<string> ExecuteStep(string stepDescription, string prompt)
    {
        string issueDescription = await _issueDescriptionProvider.GetIssueDescription();

        string fullStepPrompt = $"You are currently in step '{stepDescription}'\n" +
                                $"\n" +
                                $"{prompt}";
        
        string systemMessage = GetSystemMessage();
        string userMessage = GetFullUserInput(issueDescription, fullStepPrompt);

        return await _completions.GetCompletion(systemMessage, userMessage);
    }
    
    private string GetFullUserInput(string issueDescription, string stepInstructions)
    {
        return $"{GetMessageTop(issueDescription)}\n" +
               $"\n" +
               $"{stepInstructions}";
    }

    private string GetSystemMessage()
    {
        if (_customSystemMessage != null)
        {
            return _customSystemMessage;
        }

        return "You are an AI system that does programming tasks by reading the issue specification and modifying the existing code by " +
               "changing it or adding new code to resolve the user request. You minimize changes to the code and make sure not to modify " +
               "anything that the user has not requested. Do not remove classes, methods, or fields - only add or modify existing ones. " +
               "You will be doing the work in steps. The results of your work will be automatically put on Github. Always print " +
               "the whole source, never the 'The rest of the class remains unchanged' comment";
    }

    private string GetMessageTop(string issueDescription)
    {
        return $"The user request: \n" +
               $"{issueDescription}\n" +
               $"\n" +
               $"---\n" +
               $"\n" +
               $"{GetStepsDescription()}";
    }

    private string GetStepsDescription()
    {
        IReadOnlyList<ISolverStep> steps = _currentStepsHolder.GetCurrentSteps().Steps;

        StringBuilder stepsDescriptionBuilder = new();
        stepsDescriptionBuilder.AppendLine("Do this in steps:\n");

        int stepNumber = 1;
        
        for (int i = 0; i < steps.Count; i++)
        {
            ISolverStep step = steps[i];

            if (step.ShowInStepsListForModel == false)
            {
                continue;
            }

            stepsDescriptionBuilder.AppendLine($"{stepNumber++}. {step.DescriptionForModel}");
        }

        if (stepNumber == 1)
        {
            throw new Exception("No steps to tell the model");
        }

        return stepsDescriptionBuilder.ToString();
    }
}