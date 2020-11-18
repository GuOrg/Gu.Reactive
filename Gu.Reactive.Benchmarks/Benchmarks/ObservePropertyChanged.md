``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.572 (2004/?/20H1)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT


```
|                                 Method |        Mean |     Error |     StdDev |      Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------------------------- |------------:|----------:|-----------:|------------:|------:|--------:|-------:|------:|------:|----------:|
|               SubscribeToEventStandard |    82.89 ns |  1.689 ns |   2.255 ns |    82.80 ns |  1.00 |    0.00 | 0.0280 |     - |     - |     177 B |
|     ObservePropertyChangedSimpleLambda | 2,477.12 ns | 54.555 ns | 160.856 ns | 2,428.42 ns | 30.42 |    2.46 | 0.1640 |     - |     - |    1059 B |
|     ObservePropertyChangedNestedLambda | 2,830.39 ns | 56.608 ns | 103.511 ns | 2,817.12 ns | 34.60 |    1.42 | 0.2136 |     - |     - |    1356 B |
| ObservePropertyChangedNestedCachedPath |    71.16 ns |  1.483 ns |   2.893 ns |    70.80 ns |  0.87 |    0.05 | 0.0203 |     - |     - |     128 B |
|           ObservePropertyChangedString |   110.34 ns |  1.426 ns |   1.334 ns |   109.50 ns |  1.32 |    0.03 | 0.0203 |     - |     - |     128 B |
|       ObservePropertyChangedSlimString |   114.49 ns |  2.264 ns |   2.516 ns |   114.40 ns |  1.37 |    0.04 | 0.0203 |     - |     - |     128 B |
| ObservePropertyChangedSlimSimpleLambda | 2,387.95 ns | 47.127 ns |  91.918 ns | 2,406.96 ns | 28.99 |    1.30 | 0.1640 |     - |     - |    1059 B |
| ObservePropertyChangedSlimNestedLambda | 3,039.85 ns | 59.216 ns |  63.361 ns | 3,025.93 ns | 36.37 |    1.32 | 0.2136 |     - |     - |    1356 B |
