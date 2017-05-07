``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 6.1.7601
Processor=Intel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=8
Frequency=3410126 Hz, Resolution=293.2443 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
 |                                 Method |         Mean |      Error |     StdDev |       Median | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |--------------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|---------:|-------:|-------:|----------:|
 |               SubscribeToEventStandard |     85.10 ns |   2.110 ns |   6.019 ns |     84.04 ns |   1.00 |     0.00 | 0.0165 |      - |      88 B |
 |                       SubjectSubscribe |     56.73 ns |   1.207 ns |   1.482 ns |     56.46 ns |   0.67 |     0.05 | 0.0074 |      - |      40 B |
 |                           SimpleLambda |  2,208.82 ns |  44.229 ns | 124.748 ns |  2,177.20 ns |  26.08 |     2.30 | 0.0813 |      - |     472 B |
 |                           NestedLambda |  3,119.21 ns | 249.136 ns | 734.583 ns |  2,732.43 ns |  36.83 |     9.00 | 0.1149 |      - |     644 B |
 |      ObservePropertyChangedSimpleLamda |  3,339.72 ns |  66.601 ns | 133.010 ns |  3,335.26 ns |  39.43 |     3.10 | 0.1782 |      - |     984 B |
 | ObservePropertyChangedNestedCachedPath |  1,718.20 ns |  34.392 ns |  84.364 ns |  1,727.39 ns |  20.29 |     1.70 | 0.1640 |      - |     888 B |
 |     ObservePropertyChangedNestedLambda |  5,798.48 ns | 114.840 ns | 292.303 ns |  5,774.10 ns |  68.46 |     5.78 | 0.2768 |      - |    1532 B |
 |           ObservePropertyChangedString |    621.01 ns |  12.364 ns |  27.654 ns |    620.53 ns |   7.33 |     0.59 | 0.0905 |      - |     492 B |
 |       ObservePropertyChangedSlimString |    598.65 ns |  11.765 ns |  14.449 ns |    597.87 ns |   7.07 |     0.51 | 0.0901 |      - |     492 B |
 | ObservePropertyChangedSlimSimpleLambda |  3,237.60 ns |  64.471 ns | 131.697 ns |  3,235.05 ns |  38.23 |     3.02 | 0.1790 |      - |     984 B |
 | ObservePropertyChangedSlimNestedLambda |  5,564.58 ns | 110.336 ns | 264.357 ns |  5,527.79 ns |  65.70 |     5.44 | 0.2768 |      - |    1532 B |
 |             ObservableFromEventPattern | 10,194.21 ns | 241.498 ns | 692.903 ns | 10,038.60 ns | 120.36 |    11.55 | 0.2667 | 0.0226 |    1695 B |
 |            PropertyChangedEventManager |  3,093.79 ns |  76.444 ns | 222.992 ns |  3,095.31 ns |  36.53 |     3.61 | 0.1076 | 0.1038 |     620 B |
