namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Принадлежность контейнера
/// </summary>
public enum ContainerOwn
{
    Undefined = 0,

    /// <summary>
    /// Контейнер перевозчика
    /// </summary>
    Coc = 1,

    /// <summary>
    /// Контейнер грузоотправителя
    /// </summary>
    Soc = 2
}