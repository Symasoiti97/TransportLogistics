namespace TL.SharedKernel.Infrastructure.AspNet.Observability;

/// <summary>
/// Соглашения о телеметрии TL. Единый источник имён для кода и документации схемы логов.
/// </summary>
public static class TlTelemetry
{
    /// <summary>
    /// Все <see cref="System.Diagnostics.ActivitySource"/> проекта называются <c>TL.&lt;Контекст&gt;</c>;
    /// шаблон ниже подключает их к трейсингу автоматически.
    /// </summary>
    public const string ActivitySourcePattern = "TL.*";
}
