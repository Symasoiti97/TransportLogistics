namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Принадлежность контейнера
/// </summary>
public enum ContainerOwn
{
    Undefined,

    /// <summary>
    /// Контейнер перевозчика
    /// </summary>
    Coc,

    /// <summary>
    /// Контейнер грузоотправителя
    /// </summary>
    Soc
}