using System.Diagnostics.CodeAnalysis;
using TL.SharedKernel.Business.Aggregates;

#pragma warning disable CS8777

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

/// <summary>
/// Методы расширения для объявления ошибок
/// </summary>
public static class ErrorBuilderExtensions
{
    /// <summary>
    /// Добавляет ошибку: Тариф не найден
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="tariff">Тариф</param>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <returns>Строитель ошибок</returns>
    public static ref readonly Thrower TariffNotFoundIfNull(
        this in Thrower thrower,
        [NotNull] Tariff? tariff,
        Guid tariffId)
    {
        if (tariff is null)
        {
            Thrower.Throw(new TariffNotFound(tariffId));
        }

        return ref thrower;
    }
}