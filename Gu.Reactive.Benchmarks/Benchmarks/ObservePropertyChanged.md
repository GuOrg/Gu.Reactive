```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435865 ticks, Resolution=410.5318 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1586.0

Type=ObservePropertyChanged  Mode=Throughput  

```
                                 Method |        Median |      StdDev | Scaled |    Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------------------------- |-------------- |------------ |------- |--------- |------ |------ |------------------- |
               SubscribeToEventStandard |   112.9286 ns |   8.1086 ns |   1.00 |   115,13 |     - |     - |              25,86 |
     ObservePropertyChangedSimpleLambda | 4,997.4646 ns | 688.4558 ns |  44.25 | 1 398,00 |     - |     - |             324,38 |
     ObservePropertyChangedNestedLambda | 5,574.6778 ns | 743.7959 ns |  49.36 | 1 542,01 |     - |     - |             347,21 |
 ObservePropertyChangedNestedCachedPath |    41.8956 ns |   5.8626 ns |   0.37 |   128,33 |     - |     - |              29,05 |
           ObservePropertyChangedString |   147.4375 ns |  15.8882 ns |   1.31 |   395,64 |     - |     - |              89,31 |
       ObservePropertyChangedSlimString |   187.8106 ns |  19.8516 ns |   1.66 |   255,86 |     - |     - |              58,42 |
 ObservePropertyChangedSlimSimpleLambda | 5,312.2823 ns | 991.8562 ns |  47.04 | 1 263,34 |     - |     - |             292,30 |
 ObservePropertyChangedSlimNestedLambda | 5,975.7788 ns | 972.8024 ns |  52.92 | 1 508,93 |     - |     - |             339,66 |
