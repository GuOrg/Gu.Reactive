```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ObservePropertyChangedReact  Mode=Throughput  

```
                      Method |      Median |    StdDev | Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------------- |------------ |---------- |------- |------- |------ |------ |------------------- |
    SubscribeToEventStandard |  15.5067 ns | 0.9781 ns |   1.00 |  60,26 |     - |     - |               5,37 |
                SimpleLambda |  51.7613 ns | 2.4138 ns |   3.34 | 124,45 |     - |     - |              11,23 |
                        Slim |  36.7900 ns | 1.2539 ns |   2.37 |  54,29 |     - |     - |               4,87 |
                      Nested | 159.6342 ns | 0.9389 ns |  10.29 | 206,62 |     - |     - |              18,91 |
                          Rx |  49.5527 ns | 3.9692 ns |   3.20 | 120,15 |     - |     - |              10,85 |
 PropertyChangedEventManager | 320.5120 ns | 4.6841 ns |  20.67 | 175,00 |     - |     - |              17,04 |
