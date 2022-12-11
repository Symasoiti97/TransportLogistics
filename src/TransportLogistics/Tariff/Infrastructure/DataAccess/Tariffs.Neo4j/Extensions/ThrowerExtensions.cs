using System.Diagnostics.CodeAnalysis;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

#pragma warning disable CS8777

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Extensions;

/// <summary>
/// Методы расширения для объявления ошибок
/// </summary>
public static class ThrowerExtensions
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
        [NotNull] TariffView? tariff,
        Guid tariffId)
    {
        if (tariff is null)
        {
            Thrower.Throw(new TariffNotFound(tariffId));
        }

        return ref thrower;
    }
}