namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Route type
/// </summary>
public enum RouteType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// Road
    /// </summary>
    Auto = 2,

    /// <summary>
    /// Rail
    /// </summary>
    Railway = 3,

    /// <summary>
    /// Sea
    /// </summary>
    Sea = 4,

    /// <summary>
    /// Multimodal
    /// </summary>
    MultiModal = 5
}
