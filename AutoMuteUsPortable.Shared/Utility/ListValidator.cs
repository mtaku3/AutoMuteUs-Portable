using FluentValidation;

namespace AutoMuteUsPortable.Shared.Utility;

public class ListValidator<T> : AbstractValidator<IEnumerable<T>>
{
    public ListValidator(IValidator<T> baseValidator)
    {
        RuleForEach(x => x).SetValidator(baseValidator);
    }
}