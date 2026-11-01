using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.AspNet.ValidationAttributes;

// TODO: needs rework, validation must return the list of errors correctly
/// <summary>
/// Validation attribute that checks whether an object can be mapped to the given type
/// Uses <see cref="IMapper" /> for the mapping to the given type
/// </summary>
public class MustBeAttribute : ValidationAttribute
{
    private readonly Type _mustBeType;

    /// <summary>
    /// Creates a <see cref="MustBeAttribute" />
    /// </summary>
    /// <param name="mustBeType">Type the argument/property must be mapped to</param>
    public MustBeAttribute(Type mustBeType)
    {
        ArgumentNullException.ThrowIfNull(mustBeType);

        _mustBeType = mustBeType;
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (!TryMap(validationContext, out var message))
        {
            return new ValidationResult(message);
        }

        return ValidationResult.Success;
    }

    private bool TryMap(ValidationContext validationContext, out string? message)
    {
        message = null;
        var mapper = validationContext.GetRequiredService<IMapper>();

        try
        {
            mapper.Map(validationContext.ObjectInstance, validationContext.ObjectType, _mustBeType);
        }
        catch (ErrorException errorsException)
        {
            message = errorsException.Error.Message;
            return false;
        }

        return true;
    }
}
