```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=ThrottledView  Mode=Throughput  

```
         Method |    N |        Median |     StdDev | Scaled | Scaled-SD | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------- |----- |-------------- |----------- |------- |---------- |------ |------ |------ |------------------- |
 AddToReference | 1000 |    64.2691 us |  1.0892 us |   1.00 |      0.00 |     - |     - |     - |               0,00 |
    AddToSource | 1000 | 1,342.4189 us | 38.1423 us |  20.79 |      0.67 |     - |     - |     - |               0,00 |
      AddToView | 1000 | 1,354.1510 us | 39.7490 us |  21.01 |      0.69 |     - |     - |     - |               0,00 |
