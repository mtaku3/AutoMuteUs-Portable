using AutoMuteUs_Portable.Shared.Entity.ExecutorConfigurationSSNS;
using FluentValidation;

namespace AutoMuteUs_Portable.Core.Entity.SimpleSettingsBaseNS;

public class SimpleSettingsBaseValidator : AbstractValidator<SimpleSettingsBase>
{
    public SimpleSettingsBaseValidator()
    {
        RuleForEach(x => x.executorConfigurations).SetValidator(new ExecutorConfigurationSSValidator());
    }
}