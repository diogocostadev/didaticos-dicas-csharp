using FluentValidation;

namespace Dica80.CleanArchitecture.Application.Common;

/// <summary>
/// Base validator for commands and queries
/// </summary>
/// <typeparam name="T">Type to validate</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected void ValidateRequiredString(string propertyName, Func<T, string?> propertySelector)
    {
        RuleFor(propertySelector)
            .NotEmpty()
            .WithMessage($"{propertyName} is required")
            .MaximumLength(255)
            .WithMessage($"{propertyName} must not exceed 255 characters");
    }

    protected void ValidateRequiredEmail(Func<T, string?> propertySelector)
    {
        RuleFor(propertySelector)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address");
    }

    protected void ValidateRequiredId(string propertyName, Func<T, Guid> propertySelector)
    {
        RuleFor(propertySelector)
            .NotEmpty()
            .WithMessage($"{propertyName} is required");
    }

    protected void ValidateOptionalString(string propertyName, Func<T, string?> propertySelector, int maxLength = 255)
    {
        RuleFor(propertySelector)
            .MaximumLength(maxLength)
            .WithMessage($"{propertyName} must not exceed {maxLength} characters")
            .When(x => !string.IsNullOrEmpty(propertySelector(x)));
    }

    protected void ValidateDescription(Func<T, string?> propertySelector)
    {
        RuleFor(propertySelector)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(propertySelector(x)));
    }

    protected void ValidateEnum<TEnum>(string propertyName, Func<T, TEnum> propertySelector) where TEnum : struct, Enum
    {
        RuleFor(propertySelector)
            .IsInEnum()
            .WithMessage($"{propertyName} must be a valid value");
    }

    protected void ValidatePositiveDecimal(string propertyName, Func<T, decimal> propertySelector)
    {
        RuleFor(propertySelector)
            .GreaterThan(0)
            .WithMessage($"{propertyName} must be greater than 0");
    }

    protected void ValidateNonNegativeDecimal(string propertyName, Func<T, decimal> propertySelector)
    {
        RuleFor(propertySelector)
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{propertyName} must be greater than or equal to 0");
    }
}

/// <summary>
/// Validation behavior for MediatR pipeline
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
            
            // If TResponse is a Result type, return validation failure
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse);
                var dataType = resultType.GetGenericArguments()[0];
                var validationFailureMethod = resultType.GetMethod("ValidationFailure", new[] { typeof(List<string>) });
                
                if (validationFailureMethod != null)
                {
                    var result = validationFailureMethod.Invoke(null, new object[] { errorMessages });
                    return (TResponse)result!;
                }
            }
            else if (typeof(TResponse) == typeof(Result))
            {
                var result = Result.ValidationFailure(errorMessages);
                return (TResponse)(object)result;
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
