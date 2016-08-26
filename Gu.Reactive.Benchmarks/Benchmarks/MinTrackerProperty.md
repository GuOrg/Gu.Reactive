```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515830 ticks, Resolution=284.4279 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1076.0

Type=MinTrackerProperty  Mode=Throughput  

```
         Method |      Median |    StdDev | Scaled | Scaled-SD | Gen 0 |  Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------- |------------ |---------- |------- |---------- |------ |------- |------ |------------------- |
           Linq | 239.4370 us | 4.3575 us |   1.00 |      0.00 | 40.32 | 135.36 | 39.36 |          14 308,53 |
        Tracker | 215.0035 us | 3.6723 us |   0.90 |      0.02 | 35.27 | 117.55 | 34.29 |          12 435,50 |
 TrackerChanges | 214.9329 us | 3.4919 us |   0.89 |      0.02 | 41.00 | 133.00 | 39.00 |          14 136,97 |
