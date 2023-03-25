namespace AiProgrammer.Solving.Steps.Helpers;

public interface ICurrentStepsHolder
{
    IStepsCollection GetCurrentSteps();
    void SetCurrentSteps(IStepsCollection steps);
    void ClearCurrentSteps();
}
