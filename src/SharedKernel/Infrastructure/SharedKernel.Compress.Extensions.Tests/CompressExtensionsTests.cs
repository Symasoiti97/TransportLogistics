using FluentAssertions;

namespace TL.SharedKernel.Infrastructure.Compress.Extensions.Tests;

public class CompressExtensionsTests
{
    [Theory]
    [MemberData(nameof(CompressExtensionsTestsData.CompressTestData), MemberType = typeof(CompressExtensionsTestsData))]
    public void CompressTest(string valueToCompress, string expectedCompressedValue)
    {
        var actualCompressedValue = valueToCompress.Compress();

        actualCompressedValue.Should().Be(expectedCompressedValue);
    }

    [Theory]
    [MemberData(
        nameof(CompressExtensionsTestsData.DecompressTestData),
        MemberType = typeof(CompressExtensionsTestsData))]
    public void DecompressTest(string valueToDecompress, string expectedDecompressedValue)
    {
        var actualDecompressedValue = valueToDecompress.Decompress();

        actualDecompressedValue.Should().Be(expectedDecompressedValue);
    }
}