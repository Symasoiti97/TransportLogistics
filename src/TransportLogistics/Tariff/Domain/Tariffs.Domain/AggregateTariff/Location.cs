using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Локация
/// </summary>
public class Location : Entity<Guid>
{
    /// <summary>
    /// Тип локации
    /// </summary>
    public LocationType Type { get; private set; }

    /// <summary>
    /// Локация к которой отоносится текущая локация
    /// </summary>
    public Location? ParentLocation { get; private set; }

    public Location(Guid id, Location? parentLocation, LocationType type) : base(id)
    {
        SetParentLocation(parentLocation);
        SetType(type);
    }

    public static Location World(Guid id)
    {
        return new Location(id, null, LocationType.World);
    }

    public static Location Country(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Country);
    }

    public static Location Region(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Region);
    }

    public static Location City(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.City);
    }

    public static Location Port(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Port);
    }

    public static Location Railway(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Railway);
    }

    public static Location Terminal(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Terminal);
    }

    public static Location Warehouse(Guid id, Location parentLocation)
    {
        return new Location(id, parentLocation, LocationType.Warehouse);
    }

    private void SetType(LocationType locationType)
    {
        if (locationType == LocationType.World)
        {
            if (ParentLocation != null)
                throw new ArgumentException("ParentLocation can be null", nameof(locationType));
        }
        else
        {
            if (ParentLocation == null)
                throw new ArgumentException("ParentLocation can't be null", nameof(locationType));

            if (locationType == LocationType.Terminal)
            {
                if (ParentLocation.Type is not LocationType.Port or LocationType.Warehouse)
                    throw new ArgumentException("ParentLocation can by 'Port' or 'Warehouse'", nameof(locationType));
            }
            else
            {
                if ((int) ParentLocation.Type - 1 == (int) locationType)
                    throw new ArgumentException("Invalid ParentLocation value", nameof(locationType));
            }
        }

        Type = locationType;
    }

    private void SetParentLocation(Location? parentLocation)
    {
        ParentLocation = parentLocation;
    }
}