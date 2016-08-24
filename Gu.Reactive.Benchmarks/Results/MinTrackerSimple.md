```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=MinTrackerSimple  Mode=Throughput  

```
  Method |         Median |        StdDev | Scaled | Scaled-SD | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
-------- |--------------- |-------------- |------- |---------- |------ |------ |------ |------------------- |
    Linq | 69,443.6850 ns | 7,517.4210 ns |   1.00 |      0.00 |     - |     - |     - |              65,64 |
 Tracker |    152.8665 ns |     0.7650 ns |   0.00 |      0.00 |  2.64 |     - |  0.24 |              46,39 |
