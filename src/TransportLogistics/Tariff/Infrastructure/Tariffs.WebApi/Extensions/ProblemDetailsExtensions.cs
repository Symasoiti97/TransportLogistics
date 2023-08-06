using EnsureThat;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions;

internal static class ProblemDetailsExtensions
{
    public static void Configure(ProblemDetailsOptions options)
    {
        EnsureArg.IsNotNull(options, nameof(options));

        options.IncludeExceptionDetails = (httpContent, _) =>
            !httpContent.RequestServices.GetRequiredService<IWebHostEnvironment>().IsProduction();
        options.OnBeforeWriteDetails = (context, details) => { details.Instance = context.Request.Path; };

        options.Map<ErrorException>(
            (httpContext, errorException) =>
            {
                var serviceSettings = httpContext.RequestServices.GetRequiredService<ServiceSettings>();
                var problemDetails = new ProblemDetails
                {
                    Type = BuildType(serviceSettings.Name, errorException.Error.Type),
                    Title = errorException.Error.Message,
                    Status = GetStatusCode(errorException.Error),
                    Detail = errorException.Error.Details,
                    Extensions = {{"error", errorException.Error}}
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

        static int GetStatusCode(Error error)
        {
            return error switch
            {
                InvalidParam => StatusCodes.Status400BadRequest,
                InvalidValue => StatusCodes.Status409Conflict,
                NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }

    public static string BuildType(string serviceName, string errorType)
    {
        return $"/{serviceName}/api/errors/{errorType}";
    }
}

internal sealed class ServiceSettings
{
    public ServiceSettings(string name)
    {
        Name = name;
    }

    public string Name { get; }
}