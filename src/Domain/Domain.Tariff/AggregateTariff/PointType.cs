namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Тип точки
/// </summary>
public enum PointType
{
    Undefined,

    /// <summary>
    /// Морской транспорт
    /// </summary>
    Fob,

    /// <summary>
    /// Автомобильный транспорт
    /// </summary>
    Fot,

    /// <summary>
    /// Железодорожный транспорт
    /// </summary>
    For
}