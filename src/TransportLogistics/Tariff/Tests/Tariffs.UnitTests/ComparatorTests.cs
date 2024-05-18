using FluentAssertions;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using Xunit;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

public sealed class ComparatorTests
{
    [Theory]
    [MemberData(
        nameof(ComparatorTestsData.CompareTwoObject_Test_Data),
        MemberType = typeof(ComparatorTestsData))]
    public void CompareTwoTariffs_Test(Tariff srcTariff, Tariff destTariff, bool isEquals)
    {
        var actual = srcTariff.Equals(destTariff);

        actual.Should().Be(isEquals);
    }
}