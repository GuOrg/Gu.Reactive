```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=Get  Mode=Throughput  

```
             Method |        Median |      StdDev |   Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |-------------- |------------ |--------- |---------- |------- |------ |------ |------------------- |
               Func |     2.1530 ns |   0.0704 ns |     1.00 |      0.00 |      - |     - |     - |               0,00 |
     ValueOrDefault | 7,599.7413 ns | 110.6142 ns | 3,482.06 |    119.15 | 123.00 |     - |     - |             460,68 |
 GetValueCachedPath |   212.7670 ns |   3.3378 ns |    98.29 |      3.40 |   5.40 |     - |     - |              20,32 |
