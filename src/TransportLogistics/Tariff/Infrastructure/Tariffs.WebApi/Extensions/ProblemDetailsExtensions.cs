using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions;

internal static class ProblemDetailsExtensions
{
    public const string ErrorKey = "error";

    public static Type[] ErrorTypes
        => new[]
            {
                typeof(AssemblyReference).Assembly,
                typeof(Business.Aggregates.AssemblyReference).Assembly
            }
            .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Error))))
            .ToArray();

    public static void Configure(ProblemDetailsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.IncludeExceptionDetails = (httpContent, _) =>
            !httpContent.RequestServices.GetRequiredService<IWebHostEnvironment>().IsProduction();
        options.OnBeforeWriteDetails = (context, details) => { details.Instance = context.Request.Path; };

        var statusMapper = new ErrorToStatusMapper();
        statusMapper.AddMap<InvalidParameters>(StatusCodes.Status400BadRequest);
        statusMapper.AddMap<NotFound>(StatusCodes.Status404NotFound);
        statusMapper.AddMap<InvalidValue>(StatusCodes.Status409Conflict);
        statusMapper.AddMap<Conflict>(StatusCodes.Status409Conflict);

        options.Map<ErrorException>(
            (httpContext, errorException) =>
            {
                var serviceSettings = httpContext.RequestServices.GetRequiredService<ServiceSettings>();
                var problemDetails = new ProblemDetails
                {
                    Type = BuildType(serviceSettings.Name, errorException.Error),
                    Title = errorException.Error.Message,
                    Status = statusMapper.TryMap(errorException.Error, out var status)
                        ? status
                        : StatusCodes.Status500InternalServerError,
                    Detail = errorException.HasDetails
                        ? errorException.Message
                        : null,
                    Extensions = {{ErrorKey, errorException.Error}}
                };

                return problemDetails;
            });

        options.Map<Exception>(
            exception =>
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "about:blank",
                    Title = "Unhandle error.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message
                };

                return problemDetails;
            });
    }

    public static string BuildType(string serviceName, Error error) => $"/{serviceName}/api/errors/{error.BuildType()}";
}