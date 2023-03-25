namespace AiProgrammer.Solving.Steps.Helpers;

public class CurrentStepsHolder : ICurrentStepsHolder
{
    private IStepsCollection? _currentSteps;
    
    public IStepsCollection GetCurrentSteps()
    {
        if (_currentSteps == null)
        {
            throw new Exception("Current steps has not been set!");
        }

        return _currentSteps;
    }

    public void SetCurrentSteps(IStepsCollection steps)
    {
        if (_currentSteps != null)
        {
            throw new Exception("Could not set current steps: already set");
        }

        _currentSteps = steps ?? throw new ArgumentNullException(nameof(steps));
    }

    public void ClearCurrentSteps()
    {
        _currentSteps = null;
    }
}
