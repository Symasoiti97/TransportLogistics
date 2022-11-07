using BenchmarkDotNet.Attributes;
using Compress.Extensions;

namespace Benchmarks;

[MemoryDiagnoser]
public class CompressExtensionsBenchmark
{
    [Params(1_000, 10_000, 50_000)]
    public int N;

    private string _stringToCompress = null!;
    private string _stringToDecompress = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _stringToCompress = new string('*', N);
        _stringToDecompress = _stringToCompress.Compress();
    }

    [Benchmark]
    public void Compress()
    {
        _stringToCompress.Compress();
    }

    [Benchmark]
    public void Decompress()
    {
        _stringToDecompress.Decompress();
    }
}