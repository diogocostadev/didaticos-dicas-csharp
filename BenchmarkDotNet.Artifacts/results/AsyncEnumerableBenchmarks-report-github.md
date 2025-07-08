```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.4484)
Unknown processor
.NET SDK 9.0.201
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2


```
| Method                        | Mean             | Error         | StdDev        | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |-----------------:|--------------:|--------------:|-------:|--------:|-------:|----------:|------------:|
| TraditionalListApproach       |    61,744.446 μs |   318.5799 μs |   297.9998 μs |  1.000 |    0.01 |      - |  40.14 KB |        1.00 |
| AsyncEnumerableStreaming      |         1.997 μs |     0.0167 μs |     0.0131 μs |  0.000 |    0.00 | 0.1602 |   1.32 KB |        0.03 |
| AsyncEnumerableWithExtensions |         4.032 μs |     0.0550 μs |     0.0515 μs |  0.000 |    0.00 | 0.1755 |   1.48 KB |        0.04 |
| StreamWithTransformation      | 1,545,835.333 μs | 5,868.7483 μs | 5,489.6308 μs | 25.037 |    0.15 |      - |  32.26 KB |        0.80 |
| TraditionalTransformation     | 1,613,367.453 μs | 4,634.2279 μs | 4,334.8597 μs | 26.130 |    0.14 |      - |  71.38 KB |        1.78 |
| BatchProcessingAsync          |         2.803 μs |     0.0555 μs |     0.1055 μs |  0.000 |    0.00 | 0.2022 |   1.68 KB |        0.04 |
| TraditionalBatching           |    61,948.327 μs |   386.6242 μs |   361.6485 μs |  1.003 |    0.01 |      - |  41.85 KB |        1.04 |
| AsyncEnumerableFiltering      | 3,101,339.347 μs | 7,235.4942 μs | 6,768.0858 μs | 50.230 |    0.26 |      - |  52.79 KB |        1.31 |
