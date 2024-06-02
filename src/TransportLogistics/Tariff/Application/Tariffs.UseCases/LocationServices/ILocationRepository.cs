namespace TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

/// <summary>
/// Хранилище локаций
/// Использовать для получения локации
/// </summary>
public interface ILocationRepository
{
    /// <summary>
    /// Получить список локаций по идентификаторам
    /// </summary>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список локаций</returns>
    Task EnsureThatLocationsExists(IReadOnlySet<Guid> locationIds, CancellationToken cancellationToken);
}