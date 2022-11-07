namespace Tariffs.Infrastructure.DependencyInjection.Settings;

/// <summary>
/// Конфигурация для подключения neo4j
/// </summary>
public interface INeo4JSettings
{
    /// <summary>
    /// Адрес
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Пароль
    /// </summary>
    string Password { get; }
}