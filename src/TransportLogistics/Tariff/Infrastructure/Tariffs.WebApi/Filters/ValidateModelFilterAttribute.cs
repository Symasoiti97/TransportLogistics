using CaseExtensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Filters;

internal class ValidateModelFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            throw new ErrorException(new InvalidParams(GetErrors(context.ModelState)));
        }
    }

    private static IEnumerable<Param> GetErrors(ModelStateDictionary modelStateDictionary)
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
                    modelError.ErrorMessage);
            }
        }
    }
}