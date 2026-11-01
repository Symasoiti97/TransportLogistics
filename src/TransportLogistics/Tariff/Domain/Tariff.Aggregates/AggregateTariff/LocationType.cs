namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Location type
/// </summary>
public enum LocationType
{
    /// <summary>
    /// World
    /// </summary>
    World = 1,

    /// <summary>
    /// Country
    /// </summary>
    Country = 2,

    /// <summary>
    /// Region
    /// </summary>
    Region = 3,

    /// <summary>
    /// City
    /// </summary>
    City = 4,

    /// <summary>
    /// Port
    /// </summary>
    Port = 5,

    /// <summary>
    /// Railway station
    /// </summary>
    Railway = 6,

    /// <summary>
    /// Terminal; can belong to either a port or a railway station
    /// </summary>
    Terminal = 7,

    /// <summary>
    /// Warehouse
    /// </summary>
    Warehouse = 8
}
