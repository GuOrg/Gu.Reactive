```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ObservePropertyChangedThenSubscribeThenReact  Mode=Throughput  

```
                      Method |         Median |        StdDev |   Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |--------------- |-------------- |--------- |---------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |     84.3950 ns |     1.9714 ns |     1.00 |      0.00 |   0.87 |     - |     - |              23,90 |
                SimpleLambda | 22,282.1122 ns |   413.2215 ns |   267.21 |      7.91 |  27.43 |  4.41 |     - |             917,63 |
                        Slim |  2,172.8665 ns |    58.1266 ns |    25.79 |      0.91 |  10.73 |     - |     - |             290,58 |
                      Nested | 92,157.6817 ns | 2,104.8442 ns | 1,101.70 |     35.68 | 125.18 | 16.94 |     - |           4 154,37 |
            NestedCachedPath | 77,653.8053 ns | 1,779.2042 ns |   919.55 |     29.96 | 124.00 | 20.00 |     - |           4 432,53 |
                          Rx | 13,219.8455 ns |   311.4495 ns |   160.87 |      5.24 |  21.41 |  5.18 |     - |             806,89 |
 PropertyChangedEventManager |  3,317.7232 ns |    88.1104 ns |    39.99 |      1.39 |   1.30 |  7.09 |     - |             251,44 |
