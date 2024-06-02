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
    /// <param name="cargoEquipment">Оборудование груза</param>
    /// <param name="price">Цена</param>
    /// <param name="isDraft">True - тариф-черновик<br />False - действущий тариф</param>
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

    /// <summary>
    /// Маршрут
    /// </summary>
    public Route? Route { get; private set; }

    /// <summary>
    /// Оборудование груза
    /// </summary>
    public CargoEquipment? CargoEquipment { get; private set; }

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
    /// <param name="cargoEquipment">Оборудование груза</param>
    /// <param name="price">Цена</param>
    /// <param name="isDraft">True - тариф-черновик<br />False - действущий тариф</param>
    /// <returns>Новый тариф</returns>
    public static Tariff Create(
        Guid id,
        Guid managerProfileId,
        Route? route = null,
        CargoEquipment? cargoEquipment = null,
        Price? price = null,
        bool isDraft = true)
    {
        return new Tariff(id, managerProfileId, route, cargoEquipment, price, isDraft);
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
    /// Устанавливает оборудование груза
    /// </summary>
    public void SetCargoEquipment(CargoEquipment cargoEquipment)
    {
        ArgumentNullException.ThrowIfNull(cargoEquipment);

        CargoEquipment = cargoEquipment;
    }

    /// <summary>
    /// Устанавливает тариф как действующий
    /// </summary>
    public void SetAsReal()
    {
        if (IsDraft)
        {
            Error.Throw()
                .IfNull(Route)
                .IfNull(CargoEquipment)
                .IfNull(Price);

            IsDraft = false;
        }
    }

    private void SetManager(Guid profileId)
    {
        Error.Throw().IfDefault(profileId);

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
        if (IsDraft == false)
        {
            IsDraft = true;
        }
    }
}