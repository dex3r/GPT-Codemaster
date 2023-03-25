namespace AiProgrammer.Solving.Steps;

public interface IStepsCollection
{
    IReadOnlyList<ISolverStep> Steps { get; }
} 
