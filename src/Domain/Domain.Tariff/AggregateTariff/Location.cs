using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

public class Location : Entity<Guid>
{
    public LocationType Type { get; private set; }
    public LocalizedText Name { get; private set; }
    public Location? ParentLocation { get; private set; }

    public Location(Guid id, Location? parentLocation, LocationType type, LocalizedText name) : base(id)
    {
        SetParentLocation(parentLocation);
        SetType(type);
        //todo определится нужно ли наименование локации, или же это другой контекст
        SetName(name);
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

    private void SetName(LocalizedText name)
    {
        Name = name;
    }

    private void SetParentLocation(Location? parentLocation)
    {
        ParentLocation = parentLocation;
    }
}