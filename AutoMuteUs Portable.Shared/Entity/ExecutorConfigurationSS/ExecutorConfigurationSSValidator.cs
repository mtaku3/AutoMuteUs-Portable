using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;
using FluentValidation;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationSSNS;

public class ExecutorConfigurationSSValidator : AbstractValidator<ExecutorConfigurationSS>
{
    public ExecutorConfigurationSSValidator()
    {
        Include(new ExecutorConfigurationBaseValidator());
    }
}