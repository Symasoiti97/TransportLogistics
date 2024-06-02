using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Хранилище тарифа
/// Использовать для управления тарифом - получать и изменять тариф
/// </summary>
public interface ITariffRepository
{
    /// <summary>
    /// Получить тариф
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Тариф</returns>
    Task<Tariff> GetAsync(Guid tariffId, CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    /// <param name="cancellationToken"></param>
    Task AddAsync(Tariff tariff, CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    /// <param name="cancellationToken"></param>
    Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken);
}