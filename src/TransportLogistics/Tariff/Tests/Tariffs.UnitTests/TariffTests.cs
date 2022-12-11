using System;
using FluentAssertions;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using Xunit;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

public sealed class TariffTests
{
    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Positive_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Positive_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.CopyAsReal;

        publishTariffAction.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Negative_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Negative_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.CopyAsReal;

        publishTariffAction.Should().Throw<Exception>();
    }
}