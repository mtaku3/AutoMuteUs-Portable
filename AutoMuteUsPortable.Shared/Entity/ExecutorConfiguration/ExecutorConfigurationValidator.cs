using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using FluentValidation;

namespace AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;

public class ExecutorConfigurationValidator : AbstractValidator<ExecutorConfiguration>
{
    public ExecutorConfigurationValidator()
    {
        Include(new ExecutorConfigurationBaseValidator());
    }
}