using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Тариф
/// </summary>
public sealed class Tariff : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Маршрут
    /// </summary>
    public Route? Route { get; private set; }

    /// <summary>
    /// Груз
    /// </summary>
    public CargoType? CargoType { get; private set; }

    /// <summary>
    /// Собственность контейнера
    /// </summary>
    public ContainerOwn? ContainerOwn { get; private set; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    public ContainerSize? ContainerSize { get; private set; }

    /// <summary>
    /// Цена
    /// </summary>
    public Price? Price { get; private set; }

    /// <summary>
    /// Идентификатор профиля, менеджер текущего тарифа
    /// </summary>
    public Guid ManagerProfileId { get; private set; }

    /// <summary>
    /// Указывает тариф является черновиком или действующим
    /// True - тариф является черновиком
    /// False - тариф является действующим
    /// </summary>
    public bool IsDraft { get; private set; }

    /// <summary>
    /// Создать тариф-черновик
    /// </summary>
    /// <param name="id">Идентификатор тарифы</param>
    /// <param name="managerProfileId">Идентификатор профиля. Менеджер тарифа</param>
    /// <param name="route">Маршрут</param>
    /// <param name="containerOwn">Принадлежность контейнера</param>
    /// <param name="containerSize">Размер контейнера</param>
    /// <param name="cargoType">Тип груза</param>
    /// <param name="price">Цена</param>
    /// <param name="isDraft">True - тариф-черновик<br/>False - действущий тариф</param>
    public Tariff(
        Guid id,
        Guid managerProfileId,
        Route? route = null,
        ContainerOwn? containerOwn = null,
        ContainerSize? containerSize = null,
        CargoType? cargoType = null,
        Price? price = null,
        bool isDraft = true) : base(id)
    {
        SetManager(managerProfileId);

        if (route is not null)
        {
            SetRoute(route);
        }

        if (containerOwn != null)
        {
            SetContainerOwn(containerOwn.Value);
        }

        if (containerSize != null)
        {
            SetContainerSize(containerSize.Value);
        }

        if (cargoType != null)
        {
            SetCargoType(cargoType.Value);
        }

        if (price != null)
        {
            SetPrice(price);
        }

        SetDraft(isDraft);
    }

    public void SetPrice(Price price)
    {
        Price = price;
        SetAsDraft();
    }

    public void SetContainerSize(ContainerSize containerSize)
    {
        if (containerSize == AggregateTariff.ContainerSize.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(containerSize));

        ContainerSize = containerSize;
        SetAsDraft();
    }

    public void SetCargoType(CargoType cargoType)
    {
        if (cargoType == AggregateTariff.CargoType.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(cargoType));

        CargoType = cargoType;
        SetAsDraft();
    }

    public void SetContainerOwn(ContainerOwn containerOwn)
    {
        if (containerOwn == AggregateTariff.ContainerOwn.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(containerOwn));

        ContainerOwn = containerOwn;
        SetAsDraft();
    }

    public void SetRoute(Route route)
    {
        Route = route;
        SetAsDraft();
    }

    public void SetManager(Guid profileId)
    {
        if (Guid.Empty == profileId)
            throw new ArgumentException("Value can't be empty", nameof(profileId));

        ManagerProfileId = profileId;
        SetAsDraft();
    }

    public Tariff CopyAsReal()
    {
        var clone = (Tariff) MemberwiseClone();
        clone.SetId(Guid.NewGuid());
        clone.SetAsReal();

        return clone;
    }

    public Tariff CopyAsDraft()
    {
        var clone = (Tariff) MemberwiseClone();
        clone.SetId(Guid.NewGuid());
        clone.SetAsDraft();

        return clone;
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
        if (IsDraft == false)
        {
            IsDraft = true;
        }
    }

    private void SetAsReal()
    {
        if (IsDraft)
        {
            if (Route == null)
            {
                throw new InvalidOperationException($"{nameof(Route)} can't by empty");
            }

            if (ContainerSize.HasValue == false)
            {
                throw new InvalidOperationException($"{nameof(ContainerSize)} can't by empty");
            }

            if (ContainerOwn.HasValue == false)
            {
                throw new InvalidOperationException($"{nameof(ContainerOwn)} can't by empty");
            }

            if (Price == null)
            {
                throw new InvalidOperationException($"{nameof(Price)} can't by empty");
            }

            if (CargoType.HasValue == false)
            {
                throw new InvalidOperationException($"{nameof(CargoType)} can't by empty");
            }

            IsDraft = false;
        }
    }
}