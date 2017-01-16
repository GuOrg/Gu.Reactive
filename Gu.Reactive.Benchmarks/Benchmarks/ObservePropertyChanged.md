```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ObservePropertyChanged  Mode=Throughput  

```
                   Method |        Median |      StdDev | Scaled | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------------- |-------------- |------------ |------- |------ |------ |------ |------------------- |
 SubscribeToEventStandard |    82.5149 ns |   5.0699 ns |   1.00 |  6,79 |     - |     - |              22,74 |
             SimpleLambda | 3,816.7805 ns |  57.4402 ns |  46.26 | 75,17 |     - |     - |             260,94 |
             SimpleString |   356.4989 ns |  25.9672 ns |   4.32 | 26,56 |     - |     - |              87,23 |
               SimpleSlim |   139.4632 ns |  11.0544 ns |   1.69 | 15,00 |     - |     - |              49,24 |
             NestedLambda | 6,474.2666 ns | 581.8495 ns |  78.46 | 79,00 |     - |     - |             300,63 |
         NestedCachedPath |   416.7661 ns |  15.0816 ns |   5.05 |  5,53 |     - |     - |              20,79 |
