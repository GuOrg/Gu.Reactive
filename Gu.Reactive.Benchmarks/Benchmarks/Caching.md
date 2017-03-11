``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435887 Hz, Resolution=410.5281 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1586.0


```
 |                                        Method |            Mean |        StdErr |         StdDev |          Median | Scaled | Scaled-StdDev |   Gen 0 | Allocated |
 |---------------------------------------------- |---------------- |-------------- |--------------- |---------------- |------- |-------------- |-------- |---------- |
 |                         NewSetPoolIdentitySet |      45.3478 ns |     0.6535 ns |      3.4579 ns |      44.9463 ns |   0.06 |          0.00 |  0.0180 |      40 B |
 |         NewSetPoolIdentitySetUnionWithStrings | 304,481.3334 ns | 3,038.6675 ns | 20,609.2457 ns | 302,151.1351 ns | 376.29 |         26.28 | 24.7606 |  58.43 kB |
 |                           SetPoolBorrowReturn |     347.3898 ns |     3.7176 ns |     37.1759 ns |     344.2499 ns |   0.43 |          0.05 |       - |       7 B |
 |                                    TryTakeAdd |     809.4960 ns |     4.3232 ns |     16.7436 ns |     811.3554 ns |   1.00 |          0.00 |       - |      20 B |
 |        NewSetPoolBorrowReturnUnionWithStrings | 172,303.3268 ns | 1,508.7157 ns |  5,226.3445 ns | 172,722.4243 ns | 212.94 |          7.49 |       - |      28 B |
 |                             StringGetHashCode |      58.1138 ns |     0.6646 ns |      2.6585 ns |      58.5280 ns |   0.07 |          0.00 |       - |       0 B |
 |                            OneLevelExpression |  19,947.7656 ns |   227.4341 ns |  1,245.7081 ns |  19,768.2414 ns |  24.65 |          1.59 |  0.0732 |     473 B |
 |                            TwoLevelExpression |  25,115.7545 ns |   250.3691 ns |  2,079.7216 ns |  24,584.0067 ns |  31.04 |          2.63 |  0.2085 |     645 B |
 | PropertyPathComparerGetHashCodeSingleItemPath |     483.2592 ns |     4.8931 ns |     40.9390 ns |     465.8300 ns |   0.60 |          0.05 |       - |       0 B |
 |    PropertyPathComparerGetHashCodeTwoItemPath |     953.9795 ns |     9.6383 ns |     57.8296 ns |     934.4160 ns |   1.18 |          0.07 |       - |       0 B |
 |        NotifyingPathGetOrCreateSingleItemPath |   1,464.4515 ns |    17.4669 ns |     85.5701 ns |   1,458.9134 ns |   1.81 |          0.11 |       - |       0 B |
 |           NotifyingPathGetOrCreateTwoItemPath |   2,285.1116 ns |    22.5872 ns |    131.7046 ns |   2,268.0039 ns |   2.82 |          0.17 |       - |       0 B |
 |                     GetterGetOrCreateProperty |     279.2989 ns |     2.9850 ns |     15.7953 ns |     278.0138 ns |   0.35 |          0.02 |  0.0073 |      32 B |
