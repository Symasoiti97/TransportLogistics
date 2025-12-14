using System.ComponentModel.DataAnnotations;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public sealed class InvalidValue : Error
{
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

    public InvalidValue(object? value, string name)
    {
        Value = value;
        Name = name;
    }
}
