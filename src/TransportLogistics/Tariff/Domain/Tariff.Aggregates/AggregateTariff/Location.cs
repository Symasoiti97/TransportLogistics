using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Локация
/// </summary>
public sealed class Location : Entity<Guid>
{
    /// <summary>
    /// Создать <see cref="Location"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <param name="type">Тип локации</param>
    public Location(Guid id, Location? parentLocation, LocationType type) : base(id)
    {
        Set(type, parentLocation);
    }

    /// <summary>
    /// Тип локации
    /// </summary>
    public LocationType Type { get; private set; }

    /// <summary>
    /// Локация к которой отоносится текущая локация
    /// </summary>
    public Location? ParentLocation { get; private set; }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.World"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <returns>Локация</returns>
    public static Location World(Guid id)
    {
        return new Location(id, null, LocationType.World);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Country"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Country(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Country);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Region"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Region(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Region);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.City"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location City(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.City);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Port"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Port(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Port);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Railway"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Railway(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Railway);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Terminal"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Terminal(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Terminal);
    }

    /// <summary>
    /// Создать локацию с типом <see cref="LocationType.Warehouse"/>
    /// </summary>
    /// <param name="id">Идентификатор локации</param>
    /// <param name="parentLocation">Локация к которой отоносится текущая локация</param>
    /// <returns>Локация</returns>
    public static Location Warehouse(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Warehouse);
    }

    private void Set(LocationType locationType, Location? parentLocation)
    {
        var thrower = Error.Throw().IfUndefined(locationType);

        if (locationType == LocationType.World)
        {
            thrower.IfNotNull(parentLocation);
        }
        else
        {
            thrower.IfNull(parentLocation);

            if (locationType == LocationType.Terminal)
            {
                thrower
                    .IfNot(
                        parentLocation.Type,
                        parentLocationType => parentLocationType is LocationType.Port or LocationType.Warehouse,
                        "Parent location must be port or warehouse");
            }
            else
            {
                thrower
                    .IfNot(
                        parentLocation.Type,
                        parentLocationType => (int) parentLocationType < (int) locationType,
                        "Invalid parent location value");
            }
        }

        Type = locationType;
        ParentLocation = parentLocation;
    }
}