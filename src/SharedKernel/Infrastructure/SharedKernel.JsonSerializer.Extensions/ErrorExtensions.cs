using System.Text.Json;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

public static class ErrorExtensions
{
    public static string BuildType(this Error error)
    {
        return BuildType(error.GetType());
    }

    public static string BuildType(Type errorType)
    {
        if (!errorType.IsSubclassOf(typeof(Error)))
        {
            throw new ArgumentException("Argument errorType must be Error", nameof(errorType));
        }

        return JsonNamingPolicy.KebabCaseLower.ConvertName(errorType.Name);
    }
}