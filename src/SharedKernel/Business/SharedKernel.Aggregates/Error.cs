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

    public static implicit operator ErrorException(Error error) => new(error, message: null);

    public ErrorException WithDetails(string details, Exception? innerException = null)
        => new(this, details, innerException);
}
