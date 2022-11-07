using BenchmarkDotNet.Running;

namespace Benchmarks;

internal static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<CompressExtensionsBenchmark>();
    }
}