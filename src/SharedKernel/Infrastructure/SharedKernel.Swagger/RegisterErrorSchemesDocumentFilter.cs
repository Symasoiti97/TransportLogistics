using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

namespace TL.SharedKernel.Infrastructure.Swagger;

/// <summary>
/// Генерирует схемы для типа <see cref="Error"/>
/// </summary>
public sealed class RegisterErrorSchemesDocumentFilter : IDocumentFilter
{
    private readonly string _documentName;
    private readonly Type[] _errorTypes;

    public RegisterErrorSchemesDocumentFilter(string documentName, params Type[] errorTypes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentName);
        ArgumentNullException.ThrowIfNull(errorTypes);

        _documentName = documentName;
        _errorTypes = errorTypes;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (context.DocumentName != _documentName)
        {
            return;
        }

        foreach (var type in _errorTypes)
        {
            context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            var schema = context.SchemaRepository.Schemas[type.BuildSwaggerSchemaName()];
            var oldProperties = schema.Properties;
            schema.Properties = new Dictionary<string, OpenApiSchema>();
            schema.Properties.Add(
                new(
                    "type",
                    new OpenApiSchema
                    {
                        Type = "string",
                        ReadOnly = true,
                        Default = new OpenApiString(ErrorExtensions.BuildType(type))
                    }));
            foreach (var openApiSchema in oldProperties
                         .Where(property => property.Key is not ("message" or "details")))
            {
                schema.Properties.Add(openApiSchema);
            }
        }
    }
}