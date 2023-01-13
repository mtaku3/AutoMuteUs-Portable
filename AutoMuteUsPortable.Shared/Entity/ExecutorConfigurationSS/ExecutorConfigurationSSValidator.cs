using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationBaseNS;
using FluentValidation;

namespace AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationSSNS;

public class ExecutorConfigurationSSValidator : AbstractValidator<ExecutorConfigurationSS>
{
    public ExecutorConfigurationSSValidator()
    {
        Include(new ExecutorConfigurationBaseValidator());
    }
}