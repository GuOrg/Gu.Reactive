```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ObservePropertyChangedThenSubscribeThenReact  Mode=Throughput  

```
                      Method |         Median |        StdDev | Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |--------------- |-------------- |------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |    112.6829 ns |    29.1417 ns |   1.00 |   0,77 |     - |     - |              23,51 |
                SimpleLambda | 22,206.7433 ns |   296.7239 ns | 197.07 |  26,17 |  4,21 |     - |             978,10 |
                        Slim |  1,981.1594 ns |   169.3392 ns |  17.58 |  10,38 |     - |     - |             313,78 |
                      Nested | 89,784.7993 ns | 1,269.6301 ns | 796.79 | 126,00 | 17,00 |     - |           4 524,06 |
            NestedCachedPath | 74,509.4400 ns | 1,506.0911 ns | 661.23 | 110,30 | 17,76 |     - |           4 377,91 |
                          Rx | 13,780.0602 ns |   229.0833 ns | 122.29 |  19,09 |  4,61 |     - |             804,19 |
 PropertyChangedEventManager |  3,363.6667 ns |   224.0867 ns |  29.85 |   1,17 |  6,43 |     - |             254,06 |
