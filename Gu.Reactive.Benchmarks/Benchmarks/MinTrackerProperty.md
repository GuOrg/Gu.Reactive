```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=ACPI
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1087.0

Type=MinTrackerProperty  Mode=Throughput  

```
         Method |      Median |    StdDev | Scaled | Gen 0 |  Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------- |------------ |---------- |------- |------ |------- |------ |------------------- |
           Linq | 237.8302 us | 7.6770 us |   1.00 | 36,00 | 120,00 | 35,00 |          14 184,72 |
        Tracker | 211.1851 us | 4.4753 us |   0.89 | 35,36 | 105,11 | 31,53 |          12 770,02 |
 TrackerChanges | 218.5361 us | 4.5327 us |   0.92 | 34,00 | 113,00 | 33,00 |          13 349,20 |
