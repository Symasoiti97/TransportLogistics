namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Тип точки
/// </summary>
public enum PointType
{
    /// <summary>
    /// Морской транспорт
    /// </summary>
    Fob = 1,

    /// <summary>
    /// Автомобильный транспорт
    /// </summary>
    Fot = 2,

    /// <summary>
    /// Железодорожный транспорт
    /// </summary>
    For = 3
}