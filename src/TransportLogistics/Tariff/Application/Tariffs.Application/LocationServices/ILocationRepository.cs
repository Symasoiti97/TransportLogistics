using Application.Abstracts.Repositories;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.LocationServices;

/// <summary>
/// Хранилище локаций
/// Использовать для получения локации
/// </summary>
public interface ILocationRepository : IRepository
{
    /// <summary>
    /// Получить список локаций по идентификаторам
    /// </summary>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список локаций</returns>
    Task<Location[]> FindAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
}