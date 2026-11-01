namespace TL.SharedKernel.Infrastructure.AspNet.Options;

public sealed class ObservabilityOptions
{
    /// <summary>
    /// Base OTLP/HTTP address without the signal suffix, e.g. <c>http://localhost:5341/ingest/otlp</c>.
    /// <c>/v1/logs</c> and <c>/v1/traces</c> are appended to it.
    /// </summary>
    public string OtlpEndpoint { get; }

    /// <summary>
    /// Export headers in the <c>key=value,key2=value2</c> format (e.g. a backend API key).
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
