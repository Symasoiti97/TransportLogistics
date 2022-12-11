using TL.SharedKernel.Application.Repositories;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Хранилище тарифа
/// Использовать для управления тарифом - получать и изменять тариф
/// </summary>
public interface ITariffRepository : IRepository
{
    /// <summary>
    /// Получить тариф
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Тариф</returns>
    Task<Tariff?> FindAsync(Guid tariffId, CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    void Add(Tariff tariff);

    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    void Update(Tariff tariff);

    /// <summary>
    /// Удалить черновик тарифа
    /// </summary>
    /// <param name="tariff"></param>
    void Delete(Tariff tariff);

    /// <summary>
    /// Сохраняет изменения
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}