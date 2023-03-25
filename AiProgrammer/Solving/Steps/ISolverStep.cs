using JetBrains.Annotations;

namespace AiProgrammer.Solving.Steps;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithInheritors)]
public interface ISolverStep
{
    bool ShowInStepsListForModel { get; }
    string? DescriptionForModel { get; }
}
