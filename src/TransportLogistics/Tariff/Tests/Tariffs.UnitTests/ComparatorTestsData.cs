using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class ComparatorTestsData
{
    public static IEnumerable<object> CompareTwoObject_Test_Data()
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
        var price1 = new Price(1200, "USD");

        var srcTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price1);
        var destTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
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
        var price2 = new Price(1200, "USD");

        var srcTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price2);
        var destTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price2);

        yield return new object[]
        {
            srcTariff2, destTariff2, false
        };
    }
}