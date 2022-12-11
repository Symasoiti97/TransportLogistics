namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Тип маршрута
/// </summary>
public enum RouteType
{
    /// <summary>
    /// Неизвестный
    /// </summary>
    Unknown = 1,

    /// <summary>
    /// Авто
    /// </summary>
    Auto = 2,

    /// <summary>
    /// Железнодорожный
    /// </summary>
    Railway = 3,

    /// <summary>
    /// Морской
    /// </summary>
    Sea = 4,

    /// <summary>
    /// Мультимодальный
    /// </summary>
    MultiModal = 5
}