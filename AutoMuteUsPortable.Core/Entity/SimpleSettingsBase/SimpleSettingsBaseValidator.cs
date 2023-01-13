using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationSSNS;
using FluentValidation;

namespace AutoMuteUsPortable.Core.Entity.SimpleSettingsBaseNS;

public class SimpleSettingsBaseValidator : AbstractValidator<SimpleSettingsBase>
{
    public SimpleSettingsBaseValidator()
    {
        RuleForEach(x => x.executorConfigurations).SetValidator(new ExecutorConfigurationSSValidator());
    }
}