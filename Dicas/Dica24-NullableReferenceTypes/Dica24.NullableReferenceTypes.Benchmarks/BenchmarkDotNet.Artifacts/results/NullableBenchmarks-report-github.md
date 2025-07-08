```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.4484)
Unknown processor
.NET SDK 9.0.201
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2


```
| Method                                  | Mean         | Error        | StdDev      | Median       | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------------------------------- |-------------:|-------------:|------------:|-------------:|------:|-------:|-------:|----------:|------------:|
| ValidacaoTradicionalComTryCatch         | 920,629.2 ns | 11,259.91 ns | 9,402.54 ns | 919,924.6 ns | 1.000 | 7.8125 |      - |   72144 B |        1.00 |
| ValidacaoComNullConditional             |     527.4 ns |     10.28 ns |    11.42 ns |     522.4 ns | 0.001 |      - |      - |         - |        0.00 |
| ValidacaoComNullCheck                   |     527.5 ns |      7.49 ns |     6.26 ns |     527.3 ns | 0.001 |      - |      - |         - |        0.00 |
| ValidacaoComIsNotNull                   |     523.7 ns |      3.84 ns |     3.40 ns |     524.1 ns | 0.001 |      - |      - |         - |        0.00 |
| ProcessamentoComNullCoalescing          |   7,222.1 ns |    143.15 ns |   406.10 ns |   7,078.2 ns | 0.008 | 6.0196 | 0.6638 |   50384 B |        0.70 |
| ProcessamentoComTernario                |   7,156.7 ns |    136.50 ns |   329.66 ns |   7,035.9 ns | 0.008 | 6.0196 | 0.6638 |   50384 B |        0.70 |
| ProcessamentoUsuariosComNullConditional |   1,990.1 ns |     35.02 ns |    32.76 ns |   1,981.8 ns | 0.002 |      - |      - |         - |        0.00 |
| ProcessamentoUsuariosComNullChecks      |   1,789.8 ns |     19.74 ns |    16.48 ns |   1,789.6 ns | 0.002 |      - |      - |         - |        0.00 |
| ProcessamentoUsuariosComPatternMatching |   1,738.3 ns |     15.02 ns |    12.54 ns |   1,738.5 ns | 0.002 |      - |      - |         - |        0.00 |
| FiltrarDadosValidosComLinq              |   6,303.9 ns |     53.93 ns |    50.44 ns |   6,300.1 ns | 0.007 | 0.6561 |      - |    5544 B |        0.08 |
| FiltrarDadosValidosComLoop              |   2,884.2 ns |     51.52 ns |    43.02 ns |   2,873.6 ns | 0.003 | 1.9836 | 0.0572 |   16600 B |        0.23 |
| FiltrarDadosValidosComNullConditional   |   2,844.7 ns |     42.44 ns |    39.70 ns |   2,838.5 ns | 0.003 | 1.9836 | 0.0572 |   16600 B |        0.23 |
| ContarEmailsValidosComNullPropagation   |   1,621.3 ns |     30.41 ns |    28.45 ns |   1,613.1 ns | 0.002 |      - |      - |         - |        0.00 |
| ContarEmailsValidosComNullChecks        |   1,489.3 ns |     28.65 ns |    29.42 ns |   1,488.8 ns | 0.002 |      - |      - |         - |        0.00 |
| ConcatenarNomesComNullCoalescing        |  12,215.8 ns |    210.06 ns |   186.22 ns |  12,215.0 ns | 0.013 | 3.1433 |      - |   26408 B |        0.37 |
| ConcatenarNomesComTernario              |  11,820.7 ns |    200.30 ns |   177.56 ns |  11,850.4 ns | 0.013 | 3.1433 |      - |   26408 B |        0.37 |
| ProcessamentoDadosSemNull               |     612.6 ns |      4.79 ns |     4.25 ns |     612.6 ns | 0.001 |      - |      - |         - |        0.00 |
| ProcessamentoUsuariosSemNull            |   2,948.8 ns |     50.91 ns |    42.51 ns |   2,950.8 ns | 0.003 |      - |      - |         - |        0.00 |
