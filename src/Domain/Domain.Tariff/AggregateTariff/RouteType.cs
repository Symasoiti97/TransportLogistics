namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Тип маршрута
/// </summary>
public enum RouteType
{
    Unknown,

    /// <summary>
    /// Неопределен
    /// </summary>
    Undefined,

    /// <summary>
    /// Авто
    /// </summary>
    Auto,

    /// <summary>
    /// Железнодорожный
    /// </summary>
    Railway,

    /// <summary>
    /// Морской
    /// </summary>
    Sea,

    /// <summary>
    /// Мультимодальный
    /// </summary>
    MultiModal
}