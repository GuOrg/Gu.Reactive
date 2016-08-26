```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ObservePropertyChangedThenSubscribe  Mode=Throughput  

```
                      Method |         Median |        StdDev |   Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |--------------- |-------------- |--------- |---------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |     83.5176 ns |     1.5980 ns |     1.00 |      0.00 |   0.75 |     - |     - |              21,06 |
                 SimpleLamda | 21,301.6832 ns |   525.9040 ns |   257.75 |      7.90 |  25.29 |  4.85 |     - |             997,69 |
                SimpleString | 13,742.0806 ns |   251.1715 ns |   165.22 |      4.32 |  19.58 |  4.70 |     - |             755,63 |
                  SimpleSlim |  2,044.6057 ns |   107.8572 ns |    24.25 |      1.35 |  10.82 |     - |     - |             309,36 |
            NestedCachedPath | 75,963.0705 ns | 1,737.9611 ns |   914.00 |     26.86 | 118.00 | 19.00 |     - |           4 281,00 |
                NestedLambda | 88,084.1345 ns | 1,438.4243 ns | 1,066.58 |     26.47 | 130.23 | 17.62 |     - |           4 372,66 |
                          Rx | 13,480.8806 ns |   587.2294 ns |   164.31 |      7.58 |  18.54 |  4.44 |     - |             686,20 |
 PropertyChangedEventManager |  2,968.2650 ns |    46.5779 ns |    35.71 |      0.87 |   1.26 |  5.96 |     - |             218,05 |
