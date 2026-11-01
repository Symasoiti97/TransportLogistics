using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TL.SharedKernel.Infrastructure.AspNet.Observability;
using TL.SharedKernel.Infrastructure.AspNet.Options;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions;

public static class ObservabilityExtensions
{
    /// <summary>
    /// Enables OpenTelemetry: logs from <c>ILogger</c> and traces (HttpClient and all <c>ActivitySource</c> named <c>TL.*</c>).
    /// Works in both the generic host (workers) and Web API. For incoming HTTP requests
    /// also call <see cref="AddTlAspNetCoreInstrumentation"/>.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="options">OTLP export settings.</param>
    /// <param name="environmentName">Environment; written to the <c>Environment</c> field.</param>
    /// <param name="serviceName">Service name; written to the <c>Service</c> field in logs.</param>
    public static OpenTelemetryBuilder AddTlObservability(
        this IServiceCollection services,
        ObservabilityOptions options,
        string environmentName,
        string serviceName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";

        return services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName, serviceVersion: version)
                .AddAttributes([new KeyValuePair<string, object>("deployment.environment.name", environmentName)]))
            .WithLogging(
                logging => logging.AddOtlpExporter(exporter => Configure(exporter, options, "v1/logs")),
                logging =>
                {
                    logging.IncludeFormattedMessage = true;
                    logging.IncludeScopes = true;
                    logging.ParseStateValues = true;
                })
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddSource(TlTelemetry.ActivitySourcePattern)
                .AddOtlpExporter(exporter => Configure(exporter, options, "v1/traces")));
    }

    /// <summary>
    /// Instruments incoming HTTP requests: creates an <c>Activity</c> (the <c>TraceId</c> source)
    /// and accepts the W3C <c>traceparent</c> from the calling service.
    /// </summary>
    public static OpenTelemetryBuilder AddTlAspNetCoreInstrumentation(this OpenTelemetryBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithTracing(tracing => tracing.AddAspNetCoreInstrumentation());
    }

    private static void Configure(OtlpExporterOptions exporter, ObservabilityOptions options, string signalPath)
    {
        exporter.Endpoint = new Uri($"{options.OtlpEndpoint}/{signalPath}");
        exporter.Protocol = OtlpExportProtocol.HttpProtobuf;

        if (!string.IsNullOrWhiteSpace(options.Headers))
        {
            exporter.Headers = options.Headers;
        }
    }
}
