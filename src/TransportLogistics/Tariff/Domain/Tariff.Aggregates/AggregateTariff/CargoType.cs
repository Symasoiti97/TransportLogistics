namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Тип груза
/// </summary>
public enum CargoType
{
    /// <summary>
    /// Опасный
    /// </summary>
    Dangerous = 1,

    /// <summary>
    /// Хрупкий
    /// </summary>
    Fragile = 2,

    /// <summary>
    /// Тяжелый
    /// </summary>
    Heavy = 3,

    /// <summary>
    /// Негабаритный
    /// </summary>
    Oversize = 4,

    /// <summary>
    /// Температурный режим
    /// </summary>
    TemperatureCondition = 5,

    /// <summary>
    /// Объемный
    /// </summary>
    Bulk = 6,

    /// <summary>
    /// Стандартный груз
    /// </summary>
    Standard = 7
}