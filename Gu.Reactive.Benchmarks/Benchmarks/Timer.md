```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=Timer  Mode=Throughput  

```
      Method |        Median |     StdDev | Scaled |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------ |-------------- |----------- |------- |------- |------ |------ |------------------- |
  Observable | 3,862.7305 ns | 81.5057 ns |  12.17 | 178,00 |     - |     - |             330,18 |
  ResetTimer |   317.3099 ns |  4.5714 ns |   1.00 |  26,73 |     - |     - |              47,83 |
 ChangeTimer |   434.3272 ns | 12.5260 ns |   1.37 |  26,32 |     - |     - |              47,48 |
