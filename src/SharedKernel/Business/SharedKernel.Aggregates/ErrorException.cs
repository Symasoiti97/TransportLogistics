namespace TL.SharedKernel.Business.Aggregates;

public class ErrorException : Exception
{
    public Error Error { get; }

    public bool HasDetails { get; }

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
