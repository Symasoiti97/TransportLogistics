using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Тариф
/// </summary>
public class Tariff : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public Route Route { get; private set; } = null!;

    /// <summary>
    /// Груз
    /// </summary>
    public CargoType CargoType { get; private set; }

    /// <summary>
    /// Собственность контейнера
    /// </summary>
    public ContainerOwn ContainerOwn { get; private set; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    public ContainerSize ContainerSize { get; private set; }

    /// <summary>
    /// Цена
    /// </summary>
    public Price Price { get; private set; } = null!;

    /// <summary>
    /// Идентификатор профиля, менеджер текущего тарифа
    /// </summary>
    public Guid ManagerProfileId { get; private set; }

    public Tariff(Guid id, Guid managerProfileId, Route route, ContainerOwn containerOwn, ContainerSize containerSize, CargoType cargoType, Price price) : base(id)
    {
        SetManager(managerProfileId);
        SetRoute(route);
        SetContainerOwn(containerOwn);
        SetContainerSize(containerSize);
        SetCargoType(cargoType);
        SetPrice(price);
    }

    private void SetPrice(Price price)
    {
        Price = price;
    }

    public void SetContainerSize(ContainerSize containerSize)
    {
        if (containerSize == ContainerSize.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(containerSize));

        ContainerSize = containerSize;
    }

    public void SetCargoType(CargoType cargoType)
    {
        if (cargoType == CargoType.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(cargoType));

        CargoType = cargoType;
    }

    public void SetContainerOwn(ContainerOwn containerOwn)
    {
        if (containerOwn == ContainerOwn.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(containerOwn));

        ContainerOwn = containerOwn;
    }

    public void SetRoute(Route route)
    {
        Route = route;
    }

    public void SetManager(Guid profileId)
    {
        if (Guid.Empty == profileId)
            throw new ArgumentException("Value can't be empty", nameof(profileId));

        ManagerProfileId = profileId;
    }
}