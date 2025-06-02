using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;

public static class ApiBehaviorOptionsExtensions
{
    public static void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var error = new InvalidParameters(GetParams(actionContext.ModelState));
            var serviceSettings = actionContext.HttpContext.RequestServices.GetRequiredService<ApiSettings>();

            return new BadRequestObjectResult(
                new ProblemDetails
                {
                    Type = ProblemDetailsExtensions.BuildType(serviceSettings.Name, error),
                    Title = error.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Instance = actionContext.HttpContext.Request.Path,
                    Extensions = {{ProblemDetailsExtensions.ErrorKey, error}}
                });
        };
    }

    private static IEnumerable<InvalidParameters.Parameter> GetParams(ModelStateDictionary modelStateDictionary)
    {
        foreach (var (paramPath, modelStateEntry) in modelStateDictionary)
        {
            string? camelCaseParamName = null;
            string camelCaseParamPath;
            if (string.IsNullOrEmpty(paramPath))
            {
                camelCaseParamPath = "$";
            }
            else
            {
                var paramPathElements = paramPath
                    .Split(separator: '.')
                    .Select(pathElement => JsonNamingPolicy.CamelCase.ConvertName(pathElement))
                    .ToArray();
                camelCaseParamName = paramPathElements[^1];
                camelCaseParamPath = $"${string.Join(separator: '.', paramPathElements)}";
            }

            foreach (var modelError in modelStateEntry.Errors)
            {
                yield return new InvalidParameters.Parameter(
                    modelStateEntry.RawValue,
                    camelCaseParamName,
                    camelCaseParamPath,
                    modelError.ErrorMessage != string.Empty
                        ? modelError.ErrorMessage
                        : "Unknown error.",
                    (modelError.Exception as ErrorException)?.Error);
            }
        }
    }
}