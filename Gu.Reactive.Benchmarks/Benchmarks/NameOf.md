```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=NameOf  Mode=Throughput  

```
             Method |        Median |     StdDev |        Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |-------------- |----------- |-------------- |------- |------ |------ |------------------- |
 UsingCsharp6Nameof |     0.0002 ns |  0.0440 ns |          1.00 |      - |     - |     - |               0,00 |
           Property | 3,502.2155 ns | 98.7658 ns | 14,592,926.82 | 100,00 |     - |     - |             200,39 |
     PropertyNested | 5,622.5324 ns | 38.5531 ns | 23,427,799.91 | 173,95 |     - |     - |             348,55 |
