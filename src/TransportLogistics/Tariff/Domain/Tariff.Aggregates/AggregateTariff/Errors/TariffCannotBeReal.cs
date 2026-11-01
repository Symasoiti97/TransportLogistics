using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

/// <inheritdoc />
public sealed class TariffCannotBeReal : Conflict
{
    /// <inheritdoc />
    public override string Message => "Tariff cannot be real";

    /// <summary>
    /// Indicates whether the route is defined
    /// </summary>
    public bool IsUndefinedRoute { get; }

    /// <summary>
    /// Indicates whether the cargo equipment is defined
    /// </summary>
    public bool IsUndefinedCargoEquipment { get; }

    /// <summary>
    /// Indicates whether the price is defined
    /// </summary>
    public bool IsUndefinedPrice { get; }

    /// <inheritdoc />
    public TariffCannotBeReal(bool isUndefinedRoute, bool isUndefinedCargoEquipment, bool isUndefinedPrice)
    {
        IsUndefinedRoute = isUndefinedRoute;
        IsUndefinedCargoEquipment = isUndefinedCargoEquipment;
        IsUndefinedPrice = isUndefinedPrice;
    }
}
