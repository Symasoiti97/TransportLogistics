using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.Swagger;

/// <summary>
/// Генерирует схемы для типа <see cref="Error" />
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
            var schemaName = type.BuildSwaggerSchemaName();
            var schema = context.SchemaRepository.Schemas[schemaName];

            var newSchema = new OpenApiSchema
            {
                Type = schema.Type,
                Description = schema.Description,
                Title = schema.Title,
                Properties = new Dictionary<string, IOpenApiSchema>()
            };

            newSchema.Properties.Add("type",
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    ReadOnly = true
                });

            foreach (var property in newSchema.Properties
                         .Where(p => p.Key is not ("message" or "details")))
            {
                newSchema.Properties.Add(property.Key, property.Value);
            }

            context.SchemaRepository.Schemas[schemaName] = newSchema;
        }
    }
}
