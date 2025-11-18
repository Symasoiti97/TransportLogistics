namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Исключение об ошибкох
/// </summary>
public class ErrorException : Exception
{
    /// <summary>
    /// Ошибки
    /// </summary>
    public Error Error { get; }

    public bool HasDetails { get; }

    /// <inheritdoc />
    public ErrorException(
        Error error,
        string? message,
        Exception? innerException = null)
        : base(
            message,
            innerException)
    {
        ArgumentNullException.ThrowIfNull(error);

        Error = error;
        HasDetails = message is not null;
    }
}