using System.ComponentModel.DataAnnotations;
using AutoMapper;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.ValidationAttributes;

// TODO необходима доработка, валидация должна возвращать список ошибок корректно
/// <summary>
/// Атрибут валидации для проверки что объект может быть указанным типом
/// Для маппинга в указанный тип, используется <see cref="IMapper"/>
/// </summary>
public class MustBeAttribute : ValidationAttribute
{
    private readonly Type _mustBeType;

    /// <summary>
    /// Создать <see cref="MustBeAttribute"/>
    /// </summary>
    /// <param name="mustBeType">Тип, в который должен маппится аргумент/свойство</param>
    public MustBeAttribute(Type mustBeType)
    {
        EnsureArg.IsNotNull(mustBeType, nameof(mustBeType));

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