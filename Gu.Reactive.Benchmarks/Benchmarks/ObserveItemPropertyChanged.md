```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ObserveItemPropertyChanged  Mode=Throughput  

```
               Method |      Median |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------- |------------ |---------- |------- |------ |------ |------------------- |
            AddSimple |  99.1673 us | 2.4335 us |  90.93 |     - |     - |           5 932,71 |
            AddNested |  95.8712 us | 2.3036 us |  71.74 |     - |     - |           5 183,48 |
 AddNestedThatUpdates | 158.1318 us | 3.0260 us | 106.00 | 17.00 |     - |           8 097,56 |
