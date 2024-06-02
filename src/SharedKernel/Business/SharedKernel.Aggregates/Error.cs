namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка
/// </summary>
public abstract class Error
{
    /// <summary>
    /// Создает <see cref="Error" />
    /// </summary>
    /// <param name="details">Детали ошибки</param>
    protected Error(string details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details);

        Details = details;
    }

    /// <summary>
    /// Создает <see cref="Error" />
    /// </summary>
    protected Error()
    {
    }

    /// <summary>
    /// Детали ошибки
    /// </summary>
    public string? Details { get; }

    /// <summary>
    /// Сообщение ошибки
    /// </summary>
    public abstract string Message { get; }

    /// <summary>
    /// Инициализирует <see cref="Thrower" />
    /// </summary>
    /// <returns></returns>
    public static Thrower Throw()
    {
        return new Thrower();
    }

    public static implicit operator ErrorException(Error error) => new(error);
}