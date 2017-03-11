``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435887 Hz, Resolution=410.5281 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
 |                                        Method |           Mean |      StdErr |        StdDev |         Median | Scaled | Scaled-StdDev |  Gen 0 | Allocated |
 |---------------------------------------------- |--------------- |------------ |-------------- |--------------- |------- |-------------- |------- |---------- |
 |                         NewSetPoolIdentitySet |     44.1562 ns |   0.6296 ns |     4.5833 ns |     43.3239 ns |   1.00 |          0.00 | 0.0182 |      40 B |
 |                        NewSetPoolBorrowReturn |    309.2796 ns |   4.3327 ns |    43.3268 ns |    302.2760 ns |   7.08 |          1.22 |      - |       7 B |
 |                             StringGetHashCode |     71.9361 ns |   0.8374 ns |     5.3617 ns |     72.0573 ns |   1.65 |          0.20 |      - |       0 B |
 |                            OneLevelExpression | 18,946.2158 ns | 252.2426 ns | 2,522.4258 ns | 18,224.5241 ns | 433.51 |         72.17 | 0.0830 |     473 B |
 |                            TwoLevelExpression | 24,232.5438 ns | 263.7412 ns | 2,624.1915 ns | 23,423.1106 ns | 554.46 |         81.70 | 0.0488 |     645 B |
 | PropertyPathComparerGetHashCodeSingleItemPath |    448.8760 ns |   4.6974 ns |    46.9738 ns |    434.9284 ns |  10.27 |          1.49 |      - |       0 B |
 |    PropertyPathComparerGetHashCodeTwoItemPath |    950.8456 ns |   9.5651 ns |    95.1716 ns |    925.5951 ns |  21.76 |          3.08 |      - |       0 B |
 |        NotifyingPathGetOrCreateSingleItemPath |  1,410.0665 ns |  16.8285 ns |   168.2848 ns |  1,372.2887 ns |  32.26 |          5.02 |      - |       0 B |
 |           NotifyingPathGetOrCreateTwoItemPath |  2,247.0139 ns |  24.9100 ns |   249.1000 ns |  2,159.2203 ns |  51.41 |          7.67 |      - |       0 B |
 |                     GetterGetOrCreateProperty |    260.0580 ns |   3.1266 ns |    31.1097 ns |    250.4899 ns |   5.95 |          0.93 | 0.0125 |      32 B |
