using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class TariffTestsData
{

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var route = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), 1),
                Point.Fob(Guid.NewGuid(), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            route);

        yield return
        [
            tariff1
        ];
    }

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var route = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), 1),
                Point.Fob(Guid.NewGuid(), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            route,
            ContainerOwn.Coc,
            ContainerSize.S20,
            CargoType.Heavy,
            new Price(129, "USD"));

        yield return
        [
            tariff1
        ];
    }
}