using CaseExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions;

internal static class ApiBehaviorOptionsExtensions
{
    public static void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var error = new InvalidParams(GetParams(actionContext.ModelState));
            var serviceSettings = actionContext.HttpContext.RequestServices.GetRequiredService<ServiceSettings>();

            return new BadRequestObjectResult(
                new ProblemDetails
                {
                    Type = ProblemDetailsExtensions.BuildType(serviceSettings.Name, error.Type),
                    Title = error.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = actionContext.HttpContext.Request.Path,
                    Extensions = {{"error", error}}
                });
        };
    }

    private static IEnumerable<Param> GetParams(ModelStateDictionary modelStateDictionary)
    {
        foreach (var (paramPath, modelStateEntry) in modelStateDictionary)
        {
            string paramNameOfCamelCase;
            string paramPathOfCamelCase;
            if (string.IsNullOrEmpty(paramPath))
            {
                paramNameOfCamelCase = "$";
                paramPathOfCamelCase = "$";
            }
            else
            {
                var paramPathElements = paramPath.Split('.');
                paramNameOfCamelCase = paramPathElements[^1].ToCamelCase();
                paramPathOfCamelCase = string.Join(
                    separator: '.',
                    paramPathElements.Select(pathElement => pathElement.ToCamelCase()));
            }

            foreach (var modelError in modelStateEntry.Errors)
            {
                yield return new Param(
                    modelStateEntry.RawValue,
                    paramNameOfCamelCase,
                    paramPathOfCamelCase,
                    modelError.ErrorMessage != string.Empty
                        ? modelError.ErrorMessage
                        : "Unknown error.",
                    (modelError.Exception as ErrorException)?.Error);
            }
        }
    }
}