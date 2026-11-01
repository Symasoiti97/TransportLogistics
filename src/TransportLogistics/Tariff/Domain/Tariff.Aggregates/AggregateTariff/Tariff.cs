using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Events;
using ArgumentException = System.ComponentModel.Exceptions.ArgumentException;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

public sealed class Tariff : AggregateRoot<Guid>
{
    public Route? Route { get; private set; }

    public CargoEquipment? CargoEquipment { get; private set; }

    public Price? Price { get; private set; }

    public Guid ManagerProfileId { get; private set; }

    /// <summary>
    /// Indicates whether the tariff is a draft or active
    /// True - the tariff is a draft
    /// False - the tariff is active
    /// </summary>
    public bool IsDraft { get; private set; }

    public Tariff(
        Guid id,
        Guid managerProfileId,
        Route? route = null,
        CargoEquipment? cargoEquipment = null,
        Price? price = null,
        bool isDraft = true) : base(id)
    {
        SetManager(managerProfileId);

        if (route is not null)
        {
            SetRoute(route);
        }

        if (cargoEquipment is not null)
        {
            SetCargoEquipment(cargoEquipment);
        }

        if (price is not null)
        {
            SetPrice(price);
        }

        SetDraft(isDraft);
    }

    public static Tariff Create(
        Guid id,
        Guid managerProfileId,
        Route? route = null,
        CargoEquipment? cargoEquipment = null,
        Price? price = null,
        bool isDraft = true)
    {
        var tariff = new Tariff(id, managerProfileId, route, cargoEquipment, price, isDraft);

        tariff.Raise(TariffCreated.Create(tariff.Id));

        return tariff;
    }

    public void SetPrice(Price? price)
    {
        ArgumentNullException.ThrowIfNull(price);

        Price = price;
        SetAsDraft();
    }

    public void SetRoute(Route route)
    {
        ArgumentNullException.ThrowIfNull(route);

        Route = route;
        SetAsDraft();
    }

    public void SetCargoEquipment(CargoEquipment cargoEquipment)
    {
        ArgumentNullException.ThrowIfNull(cargoEquipment);

        CargoEquipment = cargoEquipment;
    }

    /// <summary>
    /// Sets the tariff as active
    /// </summary>
    public void SetAsReal()
    {
        if (!IsDraft)
        {
            return;
        }

        var isUndefinedRoute = Route is null;
        var isUndefinedCargoEquipment = CargoEquipment is null;
        var isUndefinedPrice = Price is null;
        if (isUndefinedRoute || isUndefinedCargoEquipment || isUndefinedPrice)
        {
            throw new TariffCannotBeReal(isUndefinedRoute, isUndefinedCargoEquipment, isUndefinedPrice);
        }

        IsDraft = false;
    }

    private void SetManager(Guid profileId)
    {
        ArgumentException.ThrowIfDefault(profileId);

        ManagerProfileId = profileId;
    }

    private void SetDraft(bool isDraft)
    {
        if (isDraft)
        {
            SetAsDraft();
        }
        else
        {
            SetAsReal();
        }
    }

    private void SetAsDraft()
    {
        if (!IsDraft)
        {
            IsDraft = true;
        }
    }
}
