```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=ObserveItemPropertyChanged  Mode=Throughput  

```
               Method |      Median |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------- |------------ |---------- |------- |------ |------ |------------------- |
            AddSimple |  76.1093 us | 1.1485 us |  78,02 |     - |     - |           4 712,14 |
            AddNested |  67.9532 us | 1.1475 us |  64,36 |     - |     - |           3 846,47 |
 AddNestedThatUpdates | 119.5901 us | 1.3259 us | 112,00 |     - |     - |           6 916,73 |
