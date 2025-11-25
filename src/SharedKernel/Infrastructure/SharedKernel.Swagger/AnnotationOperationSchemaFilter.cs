using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.SharedKernel.Infrastructure.Swagger;

/// <summary>
/// Описывает в swagger следующие аннтоции:
/// <list type="bullet">
///     <item><see cref="NotDefaultAttribute" /></item>
/// </list>
/// </summary>
public sealed class AnnotationOperationSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);

        if (schema.Properties?.Any() != true)
        {
            return;
        }

        var properties = context.Type.GetProperties().Where(t => t.HasAttribute<NotDefaultAttribute>());

        foreach (var property in properties)
        {
            if (property.PropertyType.IsValueType)
            {
                var openApiSchema = schema.Properties[JsonNamingPolicy.CamelCase.ConvertName(property.Name)];
                var additionalDescription = $"Value {Activator.CreateInstance(property.PropertyType)} is invalid";

                switch (openApiSchema.Pattern)
                {
                    case null:
                        openApiSchema.Pattern = additionalDescription;
                        break;
                    default:
                        openApiSchema.Pattern += $". {additionalDescription}";
                        break;
                }
            }
        }
    }
}
