using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Хранилище тарифа
/// Использовать для управления тарифом - получать и изменять тариф
/// </summary>
public interface ITariffRepository
{
    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CreateAsync(Tariff tariff, CancellationToken cancellationToken);

    /// <summary>
    /// Сохранить тарифа
    /// </summary>
    /// <param name="tariff">Тариф</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken);

    /// <summary>
    /// Получить тариф
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Тариф</returns>
    Task<Tariff?> FindAsync(Guid tariffId, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить черновик тарифа
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteDraftAsync(Guid tariffId, CancellationToken cancellationToken);
}