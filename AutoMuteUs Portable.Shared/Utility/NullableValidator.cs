using FluentValidation;
using FluentValidation.Results;

namespace AutoMuteUs_Portable.Shared.Utility;

// https://github.com/FluentValidation/FluentValidation/issues/1600
public class NullableValidator<TValidator> : IValidator<TValidator?> where TValidator : class
{
    private readonly IValidator<TValidator> _baseValidator;

    public NullableValidator(IValidator<TValidator> baseValidator)
    {
        _baseValidator = baseValidator;
    }

    public ValidationResult Validate(IValidationContext context)
    {
        return _baseValidator.Validate(context);
    }

    public async Task<ValidationResult> ValidateAsync(
        IValidationContext context,
        CancellationToken cancellation = new())
    {
        return await _baseValidator.ValidateAsync(context, cancellation);
    }

    public IValidatorDescriptor CreateDescriptor()
    {
        return _baseValidator.CreateDescriptor();
    }

    public bool CanValidateInstancesOfType(Type type)
    {
        return _baseValidator.CanValidateInstancesOfType(type);
    }

    public ValidationResult Validate(TValidator? instance)
    {
        return instance is null
            ? throw new InvalidOperationException("FluentValidation should not try to validate null")
            : _baseValidator.Validate(instance);
    }

    public async Task<ValidationResult> ValidateAsync(
        TValidator? instance,
        CancellationToken cancellation = new())
    {
        return instance is null
            ? throw new InvalidOperationException("FluentValidation should not try to validate null")
            : await _baseValidator.ValidateAsync(instance, cancellation);
    }
}