namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Тип локации
/// </summary>
public enum LocationType
{
    Undefined = 0,

    /// <summary>
    /// Мир
    /// </summary>
    World = 1,

    /// <summary>
    /// Страна
    /// </summary>
    Country = 2,

    /// <summary>
    /// Регион
    /// </summary>
    Region = 3,

    /// <summary>
    /// Город
    /// </summary>
    City = 4,

    /// <summary>
    /// Порт
    /// </summary>
    Port = 5,

    /// <summary>
    /// Железнодоржная станция
    /// </summary>
    Railway = 6,

    /// <summary>
    /// Терминал, может пренадлежать, как порту, так и ж/д станции
    /// </summary>
    Terminal = 7,

    /// <summary>
    /// Склад
    /// </summary>
    Warehouse = 8
}