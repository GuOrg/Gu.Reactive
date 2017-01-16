```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ThrottledView  Mode=Throughput  

```
         Method |    N |        Median |     StdDev | Scaled |  Gen 0 |  Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------- |----- |-------------- |----------- |------- |------- |------- |------ |------------------- |
 AddToReference | 1000 |    60.1432 us |  1.7285 us |   1.00 | 110,10 |      - |     - |          41 174,60 |
    AddToSource | 1000 | 1,457.7678 us | 28.7588 us |  24.24 |  56,87 | 235,37 | 45,50 |         142 862,64 |
      AddToView | 1000 | 1,388.9346 us | 52.8479 us |  23.09 |  60,00 | 215,00 | 56,00 |         137 162,16 |
