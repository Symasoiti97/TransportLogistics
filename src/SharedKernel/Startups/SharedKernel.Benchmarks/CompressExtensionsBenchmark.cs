using BenchmarkDotNet.Attributes;
using TL.SharedKernel.Infrastructure.Compress.Extensions;

namespace TL.SharedKernel.Infrastructure.Benchmarks;

[MemoryDiagnoser]
public class CompressExtensionsBenchmark
{
    private string _stringToCompress = null!;
    private string _stringToDecompress = null!;

    [Params(1_000, 10_000, 50_000)]
    public int N;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _stringToCompress = new string(c: '*', N);
        _stringToDecompress = _stringToCompress.Compress();
    }

    [Benchmark]
    public void Compress() => _stringToCompress.Compress();

    [Benchmark]
    public void Decompress() => _stringToDecompress.Decompress();
}
