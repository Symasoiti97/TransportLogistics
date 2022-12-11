using System.ComponentModel.DataAnnotations;
using EnsureThat;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public sealed class InvalidParam : Error
{
    /// <summary>
    /// Ошибачное значение
    /// </summary>
    /// <example>email.ru</example>
    public object? Value { get; }

    /// <summary>
    /// Наименование параметра
    /// </summary>
    /// <example>email</example>
    [Required]
    public string Name { get; }

    /// <summary>
    /// Путь к параметру
    /// </summary>
    /// <example>userProfile.email</example>
    [Required]
    public string Path { get; }

    /// <inheritdoc />
    public override string Message => "Invalid param.";

    /// <summary>
    /// Создать <see cref="InvalidValue"/>
    /// </summary>
    /// <param name="value">Значение параметра</param>
    /// <param name="name">Наименование параметра</param>
    /// <param name="path">Путь к параметру</param>
    /// <param name="details">Сообщение об ошибке</param>
    public InvalidParam(object? value, string name, string path, string details) : base(details)
    {
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        Value = value;
        Name = name;
        Path = path;
    }
}