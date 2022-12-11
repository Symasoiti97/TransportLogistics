using System.ComponentModel.DataAnnotations;
using EnsureThat;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public abstract class InvalidValue : Error
{
    /// <summary>
    /// Наименование значения
    /// </summary>
    /// <example>Route</example>
    [Required]
    public string Name { get; }

    /// <inheritdoc />
    protected InvalidValue(string name, string details) : base(details)
    {
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }
}

/// <inheritdoc />
public sealed class InvalidValue<TValue> : InvalidValue
{
    /// <summary>
    /// Ошибачное значение
    /// </summary>
    /// <example>null</example>
    public TValue? Value { get; }

    /// <inheritdoc />
    public override string Message => "Invalid value.";

    /// <summary>
    /// Создать <see cref="InvalidValue{TValue}"/>
    /// </summary>
    /// <param name="value">Ошибочное значение</param>
    /// <param name="name">Наименование значения</param>
    /// <param name="details">Сообщение об ошибке</param>
    public InvalidValue(TValue? value, string name, string details) : base(name, details)
    {
        Value = value;
    }
}