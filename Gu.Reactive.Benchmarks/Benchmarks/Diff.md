```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=Diff  Mode=Throughput  

```
           Method |    N |      Median |    StdDev | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
----------------- |----- |------------ |---------- |------ |------ |------ |------------------- |
 **CollectionChange** |   **10** | **124.2432 ns** | **1.8904 ns** |     **-** |     **-** |     **-** |               **0,01** |
 **CollectionChange** |  **100** | **123.8682 ns** | **2.2091 ns** |     **-** |     **-** |     **-** |               **0,01** |
 **CollectionChange** | **1000** | **124.1697 ns** | **2.0016 ns** |     **-** |     **-** |     **-** |               **0,01** |
