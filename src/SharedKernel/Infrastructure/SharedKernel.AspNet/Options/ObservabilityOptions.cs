namespace TL.SharedKernel.Infrastructure.AspNet.Options;

public sealed class ObservabilityOptions
{
    /// <summary>
    /// Базовый адрес OTLP/HTTP без суффикса сигнала, например <c>http://localhost:5341/ingest/otlp</c>.
    /// К нему добавляются <c>/v1/logs</c> и <c>/v1/traces</c>.
    /// </summary>
    public string OtlpEndpoint { get; }

    /// <summary>
    /// Заголовки экспорта в формате <c>key=value,key2=value2</c> (например, ключ API бэкенда).
    /// </summary>
    public string? Headers { get; }

    public ObservabilityOptions(string otlpEndpoint, string? headers = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(otlpEndpoint);

        if (!Uri.IsWellFormedUriString(otlpEndpoint, UriKind.Absolute))
        {
            throw new ArgumentException("OTLP endpoint must be an absolute URI.", nameof(otlpEndpoint));
        }

        OtlpEndpoint = otlpEndpoint.TrimEnd('/');
        Headers = headers;
    }
}
