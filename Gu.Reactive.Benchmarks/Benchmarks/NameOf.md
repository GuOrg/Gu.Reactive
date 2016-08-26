```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=NameOf  Mode=Throughput  

```
             Method |        Median |      StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |-------------- |------------ |------- |---------- |------- |------ |------ |------------------- |
 UsingCsharp6Nameof |     0.0014 ns |   0.0305 ns |      ? |         ? |      - |     - |     - |               0,00 |
           Property | 3,640.7350 ns | 334.2726 ns |      ? |         ? | 106.00 |     - |     - |             221,94 |
     PropertyNested | 5,529.0548 ns |  81.9919 ns |      ? |         ? | 190.67 |     - |     - |             355,13 |
