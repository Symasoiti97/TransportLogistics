using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.SharedKernel.Infrastructure.Swagger;

public sealed class ProblemDetailsSchemaDocumentFilter : IDocumentFilter
{
    private static readonly OpenApiSchema ProblemDetailsSchema =
        new()
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                {
                    "type", new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        ReadOnly = true,
                        Description = "URI identifier error",
                        Title = "Type"
                    }
                },
                {
                    "title", new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        ReadOnly = true,
                        Description = "Error message",
                        Title = "Title"
                    }
                },
                {
                    "detail", new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        ReadOnly = true,
                        Description = "Detail error message",
                        Title = "Title"
                    }
                },
                {
                    "status", new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer,
                        ReadOnly = true,
                        Description = "Status code",
                        Title = "Status code"
                    }
                },
                {
                    "instance", new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        ReadOnly = true,
                        Description = "Http route the http request",
                        Title = "Uri"
                    }
                },
                {
                    "error", new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        ReadOnly = true,
                        Description = "Error data",
                        Title = "Error",
                        AdditionalPropertiesAllowed = true,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            {
                                "type", new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    ReadOnly = true,
                                    Description = "URI identifier error",
                                    Title = "Type"
                                }
                            }
                        }
                    }
                }
            }
        };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) =>
        context.SchemaRepository.Schemas[nameof(ProblemDetails)] = ProblemDetailsSchema;
}
