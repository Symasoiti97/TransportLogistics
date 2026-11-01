namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Cargo type
/// </summary>
public enum CargoType
{
    /// <summary>
    /// Hazardous
    /// </summary>
    Dangerous = 1,

    /// <summary>
    /// Fragile
    /// </summary>
    Fragile = 2,

    /// <summary>
    /// Heavy
    /// </summary>
    Heavy = 3,

    /// <summary>
    /// Oversized
    /// </summary>
    Oversize = 4,

    /// <summary>
    /// Temperature-controlled
    /// </summary>
    TemperatureCondition = 5,

    /// <summary>
    /// Bulky
    /// </summary>
    Bulk = 6,

    /// <summary>
    /// Standard cargo
    /// </summary>
    Standard = 7
}
