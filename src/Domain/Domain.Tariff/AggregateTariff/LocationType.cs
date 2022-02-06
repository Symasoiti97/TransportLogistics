namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Тип локации
/// </summary>
public enum LocationType
{
    Undefined,

    /// <summary>
    /// Мир
    /// </summary>
    World,

    /// <summary>
    /// Страна
    /// </summary>
    Country,

    /// <summary>
    /// Регион
    /// </summary>
    Region,

    /// <summary>
    /// Город
    /// </summary>
    City,

    /// <summary>
    /// Порт
    /// </summary>
    Port,

    /// <summary>
    /// Железнодоржная станция
    /// </summary>
    Railway,

    /// <summary>
    /// Терминал, может пренадлежать, как порту, так и ж/д станции
    /// </summary>
    Terminal,

    /// <summary>
    /// Склад
    /// </summary>
    Warehouse
}