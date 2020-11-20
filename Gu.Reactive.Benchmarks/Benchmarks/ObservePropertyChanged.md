``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1198 (1909/November2018Update/19H2)
Intel Core i7-7500U CPU 2.70GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.8 (4.8.4250.0), X64 RyuJIT


```
|                                 Method |        Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------------------------- |------------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|               SubscribeToEventStandard |    95.06 ns |  1.488 ns |  1.162 ns |  1.00 |    0.00 | 0.0842 |     - |     - |     177 B |
|                 ExpressionSimpleLambda | 2,053.43 ns | 40.516 ns | 37.899 ns | 21.65 |    0.50 | 0.4234 |     - |     - |     891 B |
|                 ExpressionNestedLambda | 2,519.23 ns | 46.806 ns | 86.757 ns | 26.88 |    1.24 | 0.5836 |     - |     - |    1228 B |
|     ObservePropertyChangedSimpleLambda | 2,549.15 ns | 50.010 ns | 68.454 ns | 27.15 |    0.93 | 0.5035 |     - |     - |    1059 B |
|     ObservePropertyChangedNestedLambda | 3,177.90 ns | 57.263 ns | 89.151 ns | 33.62 |    1.26 | 0.6447 |     - |     - |    1357 B |
| ObservePropertyChangedNestedCachedPath |    84.48 ns |  1.636 ns |  4.222 ns |  0.94 |    0.06 | 0.0612 |     - |     - |     128 B |
|           ObservePropertyChangedString |   122.81 ns |  2.334 ns |  4.768 ns |  1.32 |    0.06 | 0.0610 |     - |     - |     128 B |
|       ObservePropertyChangedSlimString |   120.34 ns |  2.104 ns |  1.968 ns |  1.27 |    0.02 | 0.0612 |     - |     - |     128 B |
| ObservePropertyChangedSlimSimpleLambda | 2,592.69 ns | 51.887 ns | 71.023 ns | 27.39 |    0.93 | 0.5035 |     - |     - |    1059 B |
| ObservePropertyChangedSlimNestedLambda | 2,987.90 ns | 49.095 ns | 43.522 ns | 31.44 |    0.69 | 0.6447 |     - |     - |    1357 B |
