using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.Swagger;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions;

internal static class SwaggerGenOptionsExtensions
{
    public const string TariffApiDocumentName = "tariff-api";
    public static OpenApiInfo TariffApiInfo => new() {Title = "TL.Tariffs API", Version = "v1"};

    public const string TariffApiErrorsDocumentName = "tariff-api-errors";
    public static OpenApiInfo TariffApiErrorsInfo => new() {Title = "TL.Tariffs API Errors", Version = "v1"};

    public static void SwaggerGenOptionsAction(SwaggerGenOptions options)
    {
        options.SwaggerDoc(TariffApiDocumentName, TariffApiInfo);
        options.SwaggerDoc(TariffApiErrorsDocumentName, TariffApiErrorsInfo);

        options.SupportNonNullableReferenceTypes();
        options.UseAllOfToExtendReferenceSchemas();
        options.DocInclusionPredicate((docName, _) => docName == TariffApiDocumentName);

        foreach (var fileName in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
        {
            options.IncludeXmlComments(fileName, includeControllerXmlComments: true);
        }

        options.SchemaFilter<EnumSchemaFilter>(AppContext.BaseDirectory);
        options.SchemaFilter<AnnotationOperationSchemaFilter>();
        options.DocumentFilter<RegisterErrorSchemesDocumentFilter>(
            TariffApiErrorsDocumentName,
            new[]
            {
                typeof(ThrowerExtensions).Assembly,
                typeof(Business.Aggregates.AggregateTariff.Errors.ErrorBuilderExtensions).Assembly
            });

        options.MapType<ProblemDetails>(
            () => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    {
                        "type",
                        new OpenApiSchema
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
                        "title",
                        new OpenApiSchema
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
                        "detail",
                        new OpenApiSchema
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
                        "status",
                        new OpenApiSchema
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
                        "instance",
                        new OpenApiSchema
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
                        "error",
                        new OpenApiSchema
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
                                    "type",
                                    new OpenApiSchema
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
            });
    }
}