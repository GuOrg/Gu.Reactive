```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435865 ticks, Resolution=410.5318 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1586.0

Type=ObservePropertyChangedThenSubscribeThenReact  Mode=Throughput  

```
                                 Method |         Median |        StdDev | Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------------------------- |--------------- |-------------- |------- |------- |------ |------ |------------------- |
               SubscribeToEventStandard |    122.0663 ns |    11.8918 ns |   1.00 |  19,29 |     - |     - |              26,37 |
     ObservePropertyChangedSimpleLambda | 28,137.5011 ns | 2,495.8888 ns | 230.51 | 719,00 |     - |     - |           1 105,76 |
     ObservePropertyChangedNestedLambda | 12,872.5668 ns | 2,075.1224 ns | 105.46 | 508,27 |     - |     - |             711,78 |
 ObservePropertyChangedNestedCachedPath |  3,011.2581 ns |   494.7486 ns |  24.67 | 258,50 |     - |     - |             355,52 |
       ObservePropertyChangedSlimString |  2,836.4692 ns |   340.6218 ns |  23.24 | 255,93 |     - |     - |             351,84 |
 ObservePropertyChangedSlimSimpleLambda | 10,752.4235 ns | 1,116.3399 ns |  88.09 | 387,21 |     - |     - |             550,91 |
 ObservePropertyChangedSlimNestedLambda | 11,242.6299 ns | 1,898.2541 ns |  92.10 | 486,17 |     - |     - |             674,69 |
             ObservableFromEventPattern | 16,301.5746 ns | 2,164.6479 ns | 133.55 | 585,53 |     - |     - |             876,98 |
            PropertyChangedEventManager |  4,968.4519 ns |   505.3355 ns |  40.70 | 103,74 | 92,99 |     - |             272,25 |
