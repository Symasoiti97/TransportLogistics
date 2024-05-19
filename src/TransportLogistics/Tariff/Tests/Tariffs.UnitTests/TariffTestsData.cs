using TL.SharedKernel.Business.Aggregates.Enums;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class TariffTestsData
{

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var tariffRoute1 = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), 1),
                Point.Fob(Guid.NewGuid(), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tariffRoute1);

        yield return [tariff1];
    }

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var tariffRoute1 = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), 1),
                Point.Fob(Guid.NewGuid(), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tariffRoute1,
            new CargoEquipment(CargoType.Heavy, ContainerOwn.Coc, ContainerSize.S20),
            new Price(129, CurrencyCode.USD));

        yield return [tariff1];
    }
}