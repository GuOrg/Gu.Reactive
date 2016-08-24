```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=Timer  Mode=Throughput  

```
      Method |        Median |     StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------ |-------------- |----------- |------- |---------- |------- |------ |------ |------------------- |
  Observable | 2,002.1304 ns | 53.9061 ns |   6.48 |      0.19 | 322.00 |     - |     - |             310,41 |
  ResetTimer |   307.8301 ns |  3.8596 ns |   1.00 |      0.00 |  53.71 |     - |     - |              50,21 |
 ChangeTimer |   422.7339 ns | 51.2057 ns |   1.42 |      0.16 |  51.18 |     - |     - |              48,18 |
