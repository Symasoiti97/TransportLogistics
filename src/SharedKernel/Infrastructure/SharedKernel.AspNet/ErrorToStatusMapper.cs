using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.AspNet;

internal sealed class ErrorToStatusMapper
{
    private readonly Dictionary<Type, int> _mappers = new();

    public void AddMap<TError>(int status) where TError : Error
        => _mappers.Add(typeof(TError), status);

    internal bool TryMap(Error? error, out int status)
    {
        if (error is null)
        {
            status = default;
            return false;
        }

        foreach (var (errorType, statusCode) in _mappers)
        {
            if (errorType.IsInstanceOfType(error))
            {
                status = statusCode;
                return true;
            }
        }

        status = default;
        return false;
    }
}
