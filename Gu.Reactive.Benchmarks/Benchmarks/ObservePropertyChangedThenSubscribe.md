```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435865 ticks, Resolution=410.5318 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1586.0

Type=ObservePropertyChangedThenSubscribe  Mode=Throughput  

```
                                 Method |         Median |        StdDev | Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------------------------- |--------------- |-------------- |------- |------- |------ |------ |------------------- |
               SubscribeToEventStandard |    115.7495 ns |    13.9877 ns |   1.00 |  14,86 |     - |     - |              26,16 |
      ObservePropertyChangedSimpleLamda | 25,214.5732 ns | 1,888.5770 ns | 217.84 | 542,00 |     - |     - |             982,88 |
 ObservePropertyChangedNestedCachedPath |  2,696.4295 ns |   305.1295 ns |  23.30 | 211,65 |     - |     - |             376,54 |
     ObservePropertyChangedNestedLambda | 10,220.7793 ns |   983.6523 ns |  88.30 | 375,19 |     - |     - |             667,89 |
           ObservePropertyChangedString | 15,048.2049 ns | 1,232.3603 ns | 130.01 | 446,63 |     - |     - |             856,25 |
       ObservePropertyChangedSlimString |  2,842.8258 ns |   751.7881 ns |  24.56 | 181,22 |     - |     - |             324,87 |
 ObservePropertyChangedSlimSimpleLambda |  9,670.3425 ns |   836.4865 ns |  83.55 | 284,89 |     - |     - |             526,77 |
 ObservePropertyChangedSlimNestedLambda | 10,282.8327 ns |   924.8064 ns |  88.84 | 389,53 |     - |     - |             692,13 |
             ObservableFromEventPattern | 15,862.5845 ns | 2,264.3166 ns | 137.04 | 456,41 |     - |     - |             840,28 |
            PropertyChangedEventManager |  4,379.3335 ns |   558.2710 ns |  37.83 |  65,70 | 64,66 |  0,17 |             235,63 |
