using System.ComponentModel.DataAnnotations;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public sealed class InvalidValue : Error
{
    /// <inheritdoc />
    public override string Message => "Invalid value.";

    /// <summary>
    /// Наименование значения
    /// </summary>
    /// <example>Route</example>
    [Required]
    public string Name { get; }

    /// <summary>
    /// Ошибачное значение
    /// </summary>
    /// <example>null</example>
    public object? Value { get; }

    /// <summary>
    /// Создать <see cref="InvalidValue"/>
    /// </summary>
    /// <param name="value">Ошибочное значение</param>
    /// <param name="name">Наименование значения</param>
    /// <param name="details">Сообщение об ошибке</param>
    public InvalidValue(object? value, string name, string details) : base(details)
    {
        Value = value;
        Name = name;
    }
}