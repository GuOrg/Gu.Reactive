```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=Get  Mode=Throughput  

```
             Method |        Median |      StdDev |   Scaled | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |-------------- |------------ |--------- |------ |------ |------ |------------------- |
               Func |     2.2429 ns |   0.1084 ns |     1.00 |     - |     - |     - |               0,00 |
     ValueOrDefault | 7,806.1030 ns | 182.9904 ns | 3,480.34 | 88,00 |     - |     - |             378,92 |
 GetValueCachedPath |    63.0091 ns |   1.3281 ns |    28.09 |  1,63 |     - |     - |               6,57 |
