using TL.SharedKernel.Business.Aggregates.Enums;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class TariffTestsData
{
    public static IEnumerable<object[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var tariffRoute1 = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), order: 1),
                Point.Fob(Guid.NewGuid(), order: 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tariffRoute1);

        yield return [tariff1];
    }

    public static IEnumerable<object[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var tariffRoute1 = new Route(
            new HashSet<Point>
            {
                Point.Fob(Guid.NewGuid(), order: 1),
                Point.Fob(Guid.NewGuid(), order: 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tariffRoute1,
            new CargoEquipment(CargoType.Heavy, ContainerOwn.Coc, ContainerSize.S20),
            new Price(value: 129, CurrencyCode.USD));

        yield return [tariff1];
    }
}