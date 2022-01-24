using System.Runtime.Serialization;

namespace Shared.Exceptions;

public abstract class ApplicationException : Exception
{
    protected ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected ApplicationException(string? message) : base(message)
    {
    }

    protected ApplicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}