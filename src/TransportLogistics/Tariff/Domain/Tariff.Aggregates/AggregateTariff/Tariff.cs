using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Тариф
/// </summary>
public sealed class Tariff : Entity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Инициализировать тариф
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
    /// Создать тариф
    /// </summary>
    /// <param name="id">Идентификатор тарифы</param>
    /// <param name="managerProfileId">Идентификатор профиля. Менеджер тарифа</param>
    /// <param name="route">Маршрут</param>
    /// <param name="containerOwn">Принадлежность контейнера</param>
    /// <param name="containerSize">Размер контейнера</param>
    /// <param name="cargoType">Тип груза</param>
    /// <param name="price">Цена</param>
    /// <param name="isDraft">True - тариф-черновик<br/>False - действущий тариф</param>
    /// <returns>Новый тариф</returns>
    public static Tariff Create(
        Guid id,
        Guid managerProfileId,
        Route? route = null,
        ContainerOwn? containerOwn = null,
        ContainerSize? containerSize = null,
        CargoType? cargoType = null,
        Price? price = null,
        bool isDraft = true)
    {
        return new Tariff(id, managerProfileId, route, containerOwn, containerSize, cargoType, price, isDraft);
    }

    /// <summary>
    /// Установить цену
    /// </summary>
    /// <param name="price">Цена</param>
    public void SetPrice(Price? price)
    {
        Error.Throw().IfNull(price);

        Price = price;
        SetAsDraft();
    }

    /// <summary>
    /// Устноавить маршрут
    /// </summary>
    /// <param name="route">Маршрут</param>
    public void SetRoute(Route route)
    {
        Error.Throw().IfNull(route);

        Route = route;
        SetAsDraft();
    }

    /// <summary>
    /// Устновить менеджера
    /// </summary>
    /// <param name="profileId">Идентификатор профиля</param>
    /// <exception cref="ArgumentException">Значение идентификатор профиля пустое</exception>
    public void SetManager(Guid profileId)
    {
        Error.Throw().IfDefault(profileId);

        ManagerProfileId = profileId;
        SetAsDraft();
    }

    /// <summary>
    /// Создать копию тарифа, как действующий тариф
    /// </summary>
    /// <returns>Новая копия тарифа</returns>
    public Tariff CopyAsReal()
    {
        var clone = (Tariff) MemberwiseClone();
        clone.SetId(Guid.NewGuid());
        clone.SetAsReal();

        return clone;
    }

    /// <summary>
    /// Создать копию тарифа, как тариф-черновик
    /// </summary>
    /// <returns>Новая копия тарифа</returns>
    public Tariff CopyAsDraft()
    {
        var clone = (Tariff) MemberwiseClone();
        clone.SetId(Guid.NewGuid());
        clone.SetAsDraft();

        return clone;
    }

    /// <summary>
    /// Устанавливает оборудование груза
    /// </summary>
    public void SetCargoEquipment(ContainerSize containerSize, CargoType cargoType, ContainerOwn containerOwn)
    {
        SetContainerSize(containerSize);
        SetCargoType(cargoType);
        SetContainerOwn(containerOwn);
    }

    /// <summary>
    /// Установить размер контейнера
    /// </summary>
    /// <param name="containerSize">Размер контейнера</param>
    /// <exception cref="ArgumentException">Неопределенное значение размера контейнера</exception>
    private void SetContainerSize(ContainerSize containerSize)
    {
        Error.Throw().IfUndefined(containerSize);

        ContainerSize = containerSize;
        SetAsDraft();
    }

    /// <summary>
    /// Установить тип груза
    /// </summary>
    /// <param name="cargoType">Тип груза</param>
    /// <exception cref="ArgumentException">Неопределенное значение типа груза</exception>
    private void SetCargoType(CargoType cargoType)
    {
        Error.Throw().IfUndefined(cargoType);

        CargoType = cargoType;
        SetAsDraft();
    }

    /// <summary>
    /// Устноавить принадлежность контейнера
    /// </summary>
    /// <param name="containerOwn">Принадложность контейнера</param>
    /// <exception cref="ArgumentException">Неопределенное значение принадлежности контейнера</exception>
    private void SetContainerOwn(ContainerOwn containerOwn)
    {
        Error.Throw().IfUndefined(containerOwn);

        ContainerOwn = containerOwn;
        SetAsDraft();
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
            Error.Throw()
                .IfNull(Route)
                .IfNull(ContainerSize)
                .IfNull(ContainerOwn)
                .IfNull(Price)
                .IfNull(CargoType);

            IsDraft = false;
        }
    }
}