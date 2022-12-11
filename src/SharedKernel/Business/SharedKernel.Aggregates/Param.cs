using System.ComponentModel.DataAnnotations;
using EnsureThat;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Parameter
/// </summary>
public sealed class Param
{
    /// <summary>
    /// Creates <see cref="Param"/>
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="name">Name</param>
    /// <param name="path">Path</param>
    /// <param name="message">Error message</param>
    public Param(object? value, string name, string path, string message)
    {
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
        EnsureArg.IsNotNullOrWhiteSpace(message, nameof(message));

        Value = value;
        Name = name;
        Path = path;
        Message = message;
    }

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

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; }
}