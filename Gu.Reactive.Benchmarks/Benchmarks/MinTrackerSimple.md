```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=MinTrackerSimple  Mode=Throughput  

```
  Method |         Median |        StdDev | Scaled | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
-------- |--------------- |-------------- |------- |------ |------ |------ |------------------- |
    Linq | 68,860.5767 ns | 1,153.1306 ns |   1.00 |     - |     - |     - |              72,07 |
 Tracker |    153.6925 ns |     1.1579 ns |   0.00 |  2,47 |     - |  0,21 |              46,74 |
