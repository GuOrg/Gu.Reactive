```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ObservePropertyChanged  Mode=Throughput  

```
                   Method |        Median |      StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------------- |-------------- |------------ |------- |---------- |------- |------ |------ |------------------- |
 SubscribeToEventStandard |    81.2428 ns |   1.3446 ns |   1.00 |      0.00 |   6.30 |     - |     - |              22,47 |
             SimpleLambda | 4,155.5151 ns | 125.3290 ns |  51.13 |      1.70 |  74.75 |     - |     - |             276,08 |
             SimpleString |   366.4313 ns |   9.3352 ns |   4.44 |      0.13 |  26.08 |     - |     - |              91,18 |
               SimpleSlim |   137.0169 ns |   4.2106 ns |   1.66 |      0.06 |  14.08 |     - |     - |              49,23 |
             NestedLambda | 6,813.6044 ns | 174.9212 ns |  84.76 |      2.48 | 106.00 |     - |     - |             385,23 |
         NestedCachedPath |   434.2187 ns |   4.3428 ns |   5.32 |      0.10 |   5.94 |     - |     - |              23,73 |
