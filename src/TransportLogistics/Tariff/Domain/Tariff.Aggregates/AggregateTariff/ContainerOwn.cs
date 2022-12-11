namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Принадлежность контейнера
/// </summary>
public enum ContainerOwn
{
    /// <summary>
    /// Контейнер перевозчика
    /// </summary>
    Coc = 1,

    /// <summary>
    /// Контейнер грузоотправителя
    /// </summary>
    Soc = 2
}