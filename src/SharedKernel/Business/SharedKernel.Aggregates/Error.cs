namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка
/// </summary>
public abstract class Error
{
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

    public static implicit operator ErrorException(Error error) => new(error, message: null);

    public ErrorException WithDetails(string details) => new(this, details);
}