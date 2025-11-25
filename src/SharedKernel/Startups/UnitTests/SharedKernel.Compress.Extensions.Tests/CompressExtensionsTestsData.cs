namespace TL.SharedKernel.Infrastructure.Compress.Extensions.Tests;

public sealed class CompressExtensionsTestsData
{
    public static IEnumerable<object[]> CompressTestData()
    {
        yield return
        [
            "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
            "H4sIAAAAAAAAEzM0pC0AADssp0VgAAAA"
        ];
    }

    public static IEnumerable<object[]> DecompressTestData()
    {
        yield return
        [
            "H4sIAAAAAAAAEzM0pC0AADssp0VgAAAA",
            "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
        ];
    }
}
