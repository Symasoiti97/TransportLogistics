using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

/// <inheritdoc />
public sealed class TariffCannotBeReal : Conflict
{
    /// <inheritdoc />
    public TariffCannotBeReal(bool isUndefinedRoute, bool isUndefinedCargoEquipment, bool isUndefinedPrice)
    {
        IsUndefinedRoute = isUndefinedRoute;
        IsUndefinedCargoEquipment = isUndefinedCargoEquipment;
        IsUndefinedPrice = isUndefinedPrice;
    }

    /// <inheritdoc />
    public override string Message => "Tariff cannot be real";

    /// <summary>
    /// Указывает определен ли маршрут
    /// </summary>
    public bool IsUndefinedRoute { get; }

    /// <summary>
    /// Указывает определено ли оборудование груза
    /// </summary>
    public bool IsUndefinedCargoEquipment { get; }

    /// <summary>
    /// Указывает определена ли цена
    /// </summary>
    public bool IsUndefinedPrice { get; }
}