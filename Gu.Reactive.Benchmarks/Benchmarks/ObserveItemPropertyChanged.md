``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU X5687 3.60GHz, ProcessorCount=8
Frequency=3515820 Hz, Resolution=284.4287 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1590.0


```
 |                                                         Method |       Mean |    StdDev | Allocated |
 |--------------------------------------------------------------- |----------- |---------- |---------- |
 |               ObserveItemPropertyChangedSlimSimpleLambdaAddOne |  8.3044 us | 0.2165 us |   1.58 kB |
 |           ObserveItemPropertyChangedSlimThreeLevelLambdaAddOne | 12.7648 us | 0.1094 us |   2.28 kB |
 | ObserveItemPropertyChangedSlimThreeLevelLambdaAddOneThenUpdate | 13.5444 us | 0.1356 us |   2.36 kB |
 |                   ObserveItemPropertyChangedSimpleLambdaAddOne |  8.2538 us | 0.1722 us |   1.63 kB |
 |               ObserveItemPropertyChangedThreeLevelLambdaAddOne | 12.6882 us | 0.3096 us |   2.33 kB |
 |     ObserveItemPropertyChangedThreeLevelLambdaAddOneThenUpdate | 14.2532 us | 0.4211 us |   2.45 kB |
