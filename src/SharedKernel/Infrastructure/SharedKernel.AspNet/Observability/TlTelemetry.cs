namespace TL.SharedKernel.Infrastructure.AspNet.Observability;

/// <summary>
/// TL telemetry conventions. Single source of names for code and log schema documentation.
/// </summary>
public static class TlTelemetry
{
    /// <summary>
    /// All <see cref="System.Diagnostics.ActivitySource"/> instances in the project are named <c>TL.&lt;Context&gt;</c>;
    /// the pattern below wires them into tracing automatically.
    /// </summary>
    public const string ActivitySourcePattern = "TL.*";
}
