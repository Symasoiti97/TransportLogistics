using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TL.SharedKernel.Infrastructure.Swagger;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;

public static class SwaggerGenOptionsExtensions
{
    public static void SwaggerGenOptionsAction(ServiceSwaggerGenOptions options)
    {
        var errorsDocumentName = options.DocumentName + "-errors";
        options.SwaggerGenOptions.SwaggerDoc(
            options.DocumentName,
            new OpenApiInfo
            {
                Title = options.DocumentName,
                Version = "v1"
            });
        options.SwaggerGenOptions.SwaggerDoc(
            errorsDocumentName,
            new OpenApiInfo
            {
                Title = errorsDocumentName,
                Version = "v1"
            });

        options.SwaggerGenOptions.SupportNonNullableReferenceTypes();
        options.SwaggerGenOptions.UseAllOfToExtendReferenceSchemas();
        options.SwaggerGenOptions.DocInclusionPredicate((docName, _) => docName == options.DocumentName);

        options.SwaggerGenOptions.CustomSchemaIds(type => type.BuildSwaggerSchemaName());

        foreach (var fileName in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
        {
            options.SwaggerGenOptions.IncludeXmlComments(fileName, includeControllerXmlComments: true);
        }

        options.SwaggerGenOptions.SchemaFilter<EnumSchemaFilter>(AppContext.BaseDirectory);
        options.SwaggerGenOptions.SchemaFilter<AnnotationOperationSchemaFilter>();
        options.SwaggerGenOptions.DocumentFilter<RegisterErrorSchemesDocumentFilter>(
            errorsDocumentName,
            options.ErrorTypes);

        options.SwaggerGenOptions.MapType<ProblemDetails>(BuildProblemDetailsSchema);
    }

    private static OpenApiSchema BuildProblemDetailsSchema() =>
        new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                {
                    "type", new OpenApiSchema
                    {
                        Type = "string",
                        ReadOnly = true,
                        Description = "URI identifier error",
                        Example = new OpenApiString("/errors/not-found"),
                        Nullable = false,
                        Title = "Type"
                    }
                },
                {
                    "title", new OpenApiSchema
                    {
                        Type = "string",
                        ReadOnly = true,
                        Description = "Error message",
                        Example = new OpenApiString("Not found."),
                        Nullable = true,
                        Title = "Title"
                    }
                },
                {
                    "detail", new OpenApiSchema
                    {
                        Type = "string",
                        ReadOnly = true,
                        Description = "Detail error message",
                        Example = new OpenApiString("Entity '1' not found"),
                        Nullable = true,
                        Title = "Title"
                    }
                },
                {
                    "status", new OpenApiSchema
                    {
                        Type = "number",
                        ReadOnly = true,
                        Description = "Status code",
                        Example = new OpenApiInteger(404),
                        Nullable = true,
                        Title = "Status code"
                    }
                },
                {
                    "instance", new OpenApiSchema
                    {
                        Type = "string",
                        ReadOnly = true,
                        Description = "Http route the http request",
                        Example = new OpenApiString("/user-service/api/user/1"),
                        Nullable = true,
                        Title = "Uri"
                    }
                },
                {
                    "error", new OpenApiSchema
                    {
                        Type = "object",
                        ReadOnly = true,
                        Description = "Error data",
                        Nullable = true,
                        Title = "Error",
                        AdditionalPropertiesAllowed = true,
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            {
                                "type", new OpenApiSchema
                                {
                                    Type = "string",
                                    ReadOnly = true,
                                    Description = "URI identifier error",
                                    Example = new OpenApiString("not-found"),
                                    Nullable = false,
                                    Title = "Type"
                                }
                            }
                        }
                    }
                }
            }
        };
}