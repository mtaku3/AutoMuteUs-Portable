﻿using AutoMuteUsPortable.Core.Entity.SimpleSettingsNS;
using AutoMuteUsPortable.Shared.Entity.ExecutorConfigurationNS;
using AutoMuteUsPortable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUsPortable.Core.Entity.ConfigNS;

public class ConfigValidator : AbstractValidator<Config>
{
    public ConfigValidator()
    {
        RuleFor(x => x.serverConfiguration).SetValidator(new ServerConfigurationValidator());
    }
}

public class ServerConfigurationValidator : AbstractValidator<ServerConfiguration>
{
    public ServerConfigurationValidator()
    {
        RuleFor(x => x.simpleSettings).NotNull().When(x => x.advancedSettings == null);
        RuleFor(x => x.advancedSettings).NotNull().When(x => x.simpleSettings == null);
        RuleFor(x => x.simpleSettings)
            .SetValidator(new NullableValidator<SimpleSettings>(new SimpleSettingsValidator()));
        RuleForEach(x => x.advancedSettings).SetValidator(new ExecutorConfigurationValidator());
    }
}