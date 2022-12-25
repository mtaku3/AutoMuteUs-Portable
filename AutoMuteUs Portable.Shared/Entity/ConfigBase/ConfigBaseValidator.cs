using AutoMuteUs_Portable.Shared.Utility;
using FluentValidation;

namespace AutoMuteUs_Portable.Shared.Entity.ConfigBaseNS;

public class ConfigBaseValidator : AbstractValidator<ConfigBase>
{
    public ConfigBaseValidator()
    {
        RuleFor(x => x.executableFilePath).Must(x => Utils.ValidateFilePath(x, ".exe", true));
        RuleFor(x => x.version).Must(Utils.ValidateSHA256);
        RuleFor(x => x.logging).SetValidator(new NullableValidator<Logging>(new LoggingValidator()));
    }
}

public class LoggingValidator : AbstractValidator<Logging>
{
    public LoggingValidator()
    {
        RuleFor(x => x.outputDirectory).Must(x => Utils.ValidateDirectoryPath(x, true));
        RuleFor(x => x.logLevel).IsInEnum();
    }
}