using System.Text.Json.Serialization;
using CaseExtensions;
using EnsureThat;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка
/// </summary>
public abstract class Error
{
    /// <summary>
    /// Тип ошибки
    /// </summary>
    public string Type => GetType().Name.ToSnakeCase();

    /// <summary>
    /// Создает <see cref="Error"/>
    /// </summary>
    /// <param name="details">Детали ошибки</param>
    protected Error(string details)
    {
        EnsureArg.IsNotNullOrWhiteSpace(details, nameof(details));

        Details = details;
    }

    /// <summary>
    /// Создает <see cref="Error"/>
    /// </summary>
    protected Error()
    {
    }

    /// <summary>
    /// Детали ошибки
    /// </summary>
    [JsonIgnore]
    public string? Details { get; }

    /// <summary>
    /// Сообщение ошибки
    /// </summary>
    [JsonIgnore]
    public abstract string Message { get; }

    /// <summary>
    /// Инициализирует <see cref="Thrower"/>
    /// </summary>
    /// <returns></returns>
    public static Thrower Throw()
    {
        return new Thrower();
    }
}