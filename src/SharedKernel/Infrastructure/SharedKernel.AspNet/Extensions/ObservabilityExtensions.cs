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
    /// Подключает OpenTelemetry: логи из <c>ILogger</c> и трейсы (HttpClient и все <c>ActivitySource</c> вида <c>TL.*</c>).
    /// Работает и в generic host (воркеры), и в Web API. Для входящих HTTP-запросов
    /// дополнительно вызовите <see cref="AddTlAspNetCoreInstrumentation"/>.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="options">Настройки OTLP-экспорта.</param>
    /// <param name="environmentName">Окружение; попадает в поле <c>Environment</c>.</param>
    /// <param name="serviceName">Имя сервиса — значение поля <c>Service</c> в логах.</param>
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
    /// Инструментирует входящие HTTP-запросы: создаёт <c>Activity</c> (источник <c>TraceId</c>)
    /// и принимает W3C <c>traceparent</c> от вызывающего сервиса.
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
