using AutoMuteUs_Portable.Core.Entity.SimpleSettingsBaseNS;
using FluentValidation;

namespace AutoMuteUs_Portable.Core.Entity.SimpleSettingsNS;

public class SimpleSettingsValidator : AbstractValidator<SimpleSettings>
{
    public SimpleSettingsValidator()
    {
        Include(new SimpleSettingsBaseValidator());
        RuleFor(x => x.discordToken).NotEmpty();
        RuleFor(x => x.postgresql).SetValidator(new PostgresqlConfigurationValidator());
    }
}

public class PostgresqlConfigurationValidator : AbstractValidator<PostgresqlConfiguration>
{
    public PostgresqlConfigurationValidator()
    {
        RuleFor(x => x.username).NotEmpty();
        RuleFor(x => x.password).NotEmpty();
    }
}