namespace TL.SharedKernel.Infrastructure.Compress.Extensions.Tests;

public class CompressExtensionsTestsData
{
    public static IEnumerable<string[]> CompressTestData()
    {
        yield return new[]
        {
            "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
            "H4sIAAAAAAAACjM0pC0AADssp0VgAAAA"
        };
    }

    public static IEnumerable<string[]> DecompressTestData()
    {
        yield return new[]
        {
            "H4sIAAAAAAAACjM0pC0AADssp0VgAAAA",
            "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
        };
    }
}