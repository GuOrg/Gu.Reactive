```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ObservePropertyChangedReact  Mode=Throughput  

```
                      Method |      Median |    StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |------------ |---------- |------- |---------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |  15.1554 ns | 0.7688 ns |   1.00 |      0.00 |  45.34 |     - |     - |               4,71 |
                SimpleLambda |  55.0129 ns | 3.9144 ns |   3.72 |      0.33 | 110.52 |     - |     - |              11,59 |
                        Slim |  36.1676 ns | 1.0180 ns |   2.46 |      0.15 |  49.13 |     - |     - |               5,12 |
                      Nested | 149.7411 ns | 3.4859 ns |  10.09 |      0.59 | 199.27 |     - |     - |              21,17 |
                          Rx |  48.6520 ns | 1.7345 ns |   3.30 |      0.21 | 114.29 |     - |     - |              11,94 |
 PropertyChangedEventManager | 313.8882 ns | 6.5846 ns |  21.08 |      1.21 | 175.00 |     - |     - |              19,81 |
