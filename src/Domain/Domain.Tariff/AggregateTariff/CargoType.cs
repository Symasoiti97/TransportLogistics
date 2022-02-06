namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Тип груза
/// </summary>
public enum CargoType
{
    Undefined,

    /// <summary>
    /// Опасный
    /// </summary>
    Dangerous,

    /// <summary>
    /// Хрупкий
    /// </summary>
    Fragile,

    /// <summary>
    /// Тяжелый
    /// </summary>
    Heavy,

    /// <summary>
    /// Негабаритный
    /// </summary>
    Oversize,

    /// <summary>
    /// Температурный режим
    /// </summary>
    TemperatureCondition,

    /// <summary>
    /// Объемный
    /// </summary>
    Bulk,

    /// <summary>
    /// Стандартный груз
    /// </summary>
    Standard
}