using System.Reflection;
using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.Swagger;

/// <summary>
/// Генерирует схемы для типа <see cref="Error"/>
/// </summary>
public sealed class RegisterErrorSchemesDocumentFilter : IDocumentFilter
{
    private readonly string _documentName;
    private readonly Assembly[] _assemblies;

    public RegisterErrorSchemesDocumentFilter(string documentName, params Assembly[] assemblies)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentName);
        ArgumentNullException.ThrowIfNull(assemblies);

        _documentName = documentName;
        _assemblies = assemblies;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (context.DocumentName != _documentName)
        {
            return;
        }

        var baseType = typeof(Error);
        foreach (var assembly in _assemblies)
        {
            foreach (var type in assembly.GetTypes().Where(type => type.IsSubclassOf(baseType)))
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
                context.SchemaRepository
                    .Schemas[type.BuildSwaggerSchemaName()]
                    .Properties[JsonNamingPolicy.CamelCase.ConvertName(nameof(Error.Type))]
                    .Default = new OpenApiString(JsonNamingPolicy.SnakeCaseLower.ConvertName(type.Name));
            }
        }
    }
}