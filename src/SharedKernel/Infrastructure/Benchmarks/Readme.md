**BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)<br/>
12th Gen Intel Core i5-12600K, 1 CPU, 16 logical and 10 physical cores<br/>
.NET SDK=6.0.402<br/>
[Host]: .NET 6.0.10 (6.0.1022.47605), X64 RyuJIT AVX2**


|     Method |     N |      Mean |     Error |    StdDev |    Gen0 |    Gen1 |    Gen2 | Allocated |
|----------- |------ |----------:|----------:|----------:|--------:|--------:|--------:|----------:|
|   Compress |  1000 |  5.480 us | 0.0983 us | 0.0920 us |  0.1678 |       - |       - |   1.73 KB |
| Decompress |  1000 |  1.055 us | 0.0118 us | 0.0110 us |  0.4368 |  0.0019 |       - |   4.46 KB |
|   Compress | 10000 |  8.416 us | 0.1675 us | 0.4615 us |  1.0223 |       - |       - |  10.58 KB |
| Decompress | 10000 |  5.833 us | 0.0607 us | 0.0538 us |  3.8757 |       - |       - |  39.63 KB |
|   Compress | 50000 | 20.647 us | 0.4065 us | 0.9502 us |  4.8218 |       - |       - |  49.77 KB |
| Decompress | 50000 | 46.849 us | 0.7528 us | 0.7041 us | 32.1655 | 32.1655 | 32.1655 | 199.94 KB |
