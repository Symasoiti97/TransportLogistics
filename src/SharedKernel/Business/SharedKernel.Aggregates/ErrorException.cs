using EnsureThat;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Исключение об ошибкох
/// </summary>
public class ErrorException : Exception
{
    /// <inheritdoc />
    public ErrorException(
        Error error,
        Exception? innerException = null)
        : base(
            error.Message,
            innerException)
    {
        EnsureArg.IsNotNull(error, nameof(error));

        Error = error;
    }

    /// <summary>
    /// Ошибки
    /// </summary>
    public Error Error { get; }
}