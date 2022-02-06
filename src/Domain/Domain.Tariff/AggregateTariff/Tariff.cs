using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

public class Tariff : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public Route Route { get; private set; }

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
    public Price Price { get; private set; }

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
        ContainerSize = containerSize;
    }

    public void SetCargoType(CargoType cargoType)
    {
        CargoType = cargoType;
    }

    public void SetContainerOwn(ContainerOwn containerOwn)
    {
        ContainerOwn = containerOwn;
    }

    public void SetRoute(Route route)
    {
        Route = route;
    }

    public void SetManager(Guid profileId)
    {
        if (Guid.Empty == profileId)
            throw new ArgumentException("Can be not empty", nameof(profileId));

        ManagerProfileId = profileId;
    }
}