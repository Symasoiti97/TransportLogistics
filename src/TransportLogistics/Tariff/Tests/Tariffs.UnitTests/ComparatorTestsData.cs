using TL.SharedKernel.Business.Aggregates.Enums;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class ComparatorTestsData
{
    public static IEnumerable<object> CompareTariffs_Test_Data()
    {
        var tariffId1 = Guid.NewGuid();
        var managerProfileId1 = Guid.NewGuid();

        var firstCountryLocationId1 = Guid.NewGuid();
        var secondCountryLocationId1 = Guid.NewGuid();

        var points1 = new HashSet<Point>
        {
            Point.Fot(firstCountryLocationId1, 1),
            Point.Fot(secondCountryLocationId1, 2)
        };
        var route1 = new Route(points1);
        var price1 = new Price(1200, CurrencyCode.USD);

        var srcTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            new CargoEquipment(CargoType.Bulk, ContainerOwn.Soc, ContainerSize.S20),
            price1);
        var destTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            new CargoEquipment(CargoType.Bulk, ContainerOwn.Soc, ContainerSize.S20),
            price1);

        yield return new object[]
        {
            srcTariff1, destTariff1, true
        };

        var managerProfileId2 = Guid.NewGuid();

        var firstCountryLocationId2 = Guid.NewGuid();
        var secondCountryLocationId2 = Guid.NewGuid();

        var points2 = new HashSet<Point>
        {
            Point.Fot(firstCountryLocationId2, 1),
            Point.Fot(secondCountryLocationId2, 2)
        };
        var route2 = new Route(points2);
        var price2 = new Price(1200, CurrencyCode.USD);

        var srcTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            new CargoEquipment(CargoType.Bulk, ContainerOwn.Soc, ContainerSize.S20),
            price2);
        var destTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            new CargoEquipment(CargoType.Bulk, ContainerOwn.Soc, ContainerSize.S20),
            price2);

        yield return new object[]
        {
            srcTariff2, destTariff2, false
        };
    }

    public static IEnumerable<object> CompareRoutes_Test_Data()
    {
        var srcPoints1 = new HashSet<Point>
        {
            Point.Fot(locationId: Guid.NewGuid(), order: 1),
            Point.Fot(locationId: Guid.NewGuid(), order: 2)
        };
        var destPoints1 = new HashSet<Point>
        {
            Point.Fot(locationId: Guid.NewGuid(), order: 1),
            Point.Fot(locationId: Guid.NewGuid(), order: 2)
        };
        var srcRoute1 = new Route(srcPoints1);
        var destRoute1 = new Route(destPoints1);

        yield return new object[]
        {
            srcRoute1, destRoute1, false
        };

        var firstCountryLocationId2 = Guid.NewGuid();
        var secondCountryLocationId2 = Guid.NewGuid();

        var srcPoints2 = new HashSet<Point>
        {
            Point.Fot(firstCountryLocationId2, 1),
            Point.Fot(secondCountryLocationId2, 2)
        };
        var destPoints2 = new HashSet<Point>
        {
            Point.Fot(firstCountryLocationId2, order: 1),
            Point.Fot(secondCountryLocationId2, order: 2)
        };
        var srcRoute2 = new Route(srcPoints2);
        var destRoute2 = new Route(destPoints2);

        yield return new object[]
        {
            srcRoute2, destRoute2, true
        };

        var srcPoints3 = new HashSet<Point>
        {
            Point.Fot(secondCountryLocationId2, 2),
            Point.Fot(firstCountryLocationId2, 1)
        };
        var destPoints3 = new HashSet<Point>
        {
            Point.Fot(firstCountryLocationId2, 1),
            Point.Fot(secondCountryLocationId2, 2)
        };
        var srcRoute3 = new Route(srcPoints3);
        var destRoute3 = new Route(destPoints3);

        yield return new object[]
        {
            srcRoute3, destRoute3, true
        };
    }
}