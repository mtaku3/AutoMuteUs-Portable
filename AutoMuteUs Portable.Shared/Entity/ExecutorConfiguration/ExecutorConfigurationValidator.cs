using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationBaseNS;
using FluentValidation;

namespace AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationNS;

public class ExecutorConfigurationValidator : AbstractValidator<ExecutorConfiguration>
{
    public ExecutorConfigurationValidator()
    {
        Include(new ExecutorConfigurationBaseValidator());
    }
}