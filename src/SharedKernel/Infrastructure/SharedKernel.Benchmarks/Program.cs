using BenchmarkDotNet.Running;

namespace TL.SharedKernel.Infrastructure.Benchmarks;

internal static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<CompressExtensionsBenchmark>();
    }
}