``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 Hz, Resolution=410.5309 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
 |                                        Method |        Mean |    StdDev | Scaled | Scaled-StdDev |  Gen 0 | Allocated |
 |---------------------------------------------- |------------ |---------- |------- |-------------- |------- |---------- |
 |                             StringGetHashCode |  10.8353 ns | 0.1403 ns |   1.00 |          0.00 |      - |       0 B |
 | PropertyPathComparerGetHashCodeSingleItemPath |  94.5422 ns | 1.0563 ns |   8.73 |          0.14 |      - |       0 B |
 |    PropertyPathComparerGetHashCodeTwoItemPath | 203.0941 ns | 0.6008 ns |  18.75 |          0.24 |      - |       0 B |
 |        NotifyingPathGetOrCreateSingleItemPath | 284.7353 ns | 0.9277 ns |  26.28 |          0.34 |      - |       0 B |
 |           NotifyingPathGetOrCreateTwoItemPath | 461.9358 ns | 5.6113 ns |  42.64 |          0.73 |      - |       0 B |
 |                     GetterGetOrCreateProperty |  50.7808 ns | 0.5496 ns |   4.69 |          0.08 | 0.0119 |      32 B |
