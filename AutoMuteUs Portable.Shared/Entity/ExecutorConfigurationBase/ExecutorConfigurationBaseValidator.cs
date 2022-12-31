using AutoMuteUs_Portable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;

public class ExecutorConfigurationBaseValidator : AbstractValidator<ExecutorConfigurationBase>
{
    public ExecutorConfigurationBaseValidator()
    {
        RuleFor(x => x.version).NotEmpty();
        RuleFor(x => x.type).IsInEnum();
        RuleFor(x => x.binaryVersion).NotEmpty();
        RuleFor(x => x.binaryDirectory).Must(x => Utils.ValidateDirectoryPath(x));
    }
}