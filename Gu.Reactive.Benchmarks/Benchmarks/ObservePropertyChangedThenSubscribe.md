```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ObservePropertyChangedThenSubscribe  Mode=Throughput  

```
                      Method |         Median |         StdDev |   Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |--------------- |--------------- |--------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |     86.2522 ns |      1.4457 ns |     1.00 |   0,79 |     - |     - |              24,26 |
                 SimpleLamda | 22,216.2306 ns |    499.6560 ns |   257.57 |  23,65 |  4,54 |     - |           1 020,53 |
                SimpleString | 14,333.6086 ns |    551.8544 ns |   166.18 |  18,98 |  4,57 |     - |             800,22 |
                  SimpleSlim |  1,950.8411 ns |     70.0890 ns |    22.62 |   8,09 |     - |     - |             253,32 |
            NestedCachedPath | 87,376.1203 ns | 12,982.5133 ns | 1,013.03 | 100,00 | 16,00 |     - |           3 955,30 |
                NestedLambda | 86,658.1066 ns |  2,339.4751 ns | 1,004.71 | 123,39 | 16,83 |     - |           4 428,72 |
                          Rx | 13,697.9426 ns |    358.5167 ns |   158.81 |  19,64 |  4,75 |     - |             792,02 |
 PropertyChangedEventManager |  2,987.4647 ns |     98.6367 ns |    34.64 |   1,16 |  5,31 |     - |             215,57 |
