```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=Diff  Mode=Throughput  

```
           Method |    N |      Median |    StdDev | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
----------------- |----- |------------ |---------- |------ |------ |------ |------------------- |
 **CollectionChange** |   **10** | **131.4031 ns** | **2.2953 ns** |     **-** |     **-** |     **-** |               **0,01** |
 **CollectionChange** |  **100** | **123.9039 ns** | **3.4183 ns** |     **-** |     **-** |     **-** |               **0,01** |
 **CollectionChange** | **1000** | **129.4650 ns** | **2.2220 ns** |     **-** |     **-** |     **-** |               **0,01** |
