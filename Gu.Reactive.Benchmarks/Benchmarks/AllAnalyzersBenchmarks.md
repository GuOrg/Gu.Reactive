``` ini

BenchmarkDotNet=v0.10.10, OS=Windows 7 SP1 (6.1.7601.0)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410117 Hz, Resolution=293.2451 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2116.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2116.0


```
|                                 Method |           Mean |         Error |        StdDev |         Median |  Gen 0 |  Gen 1 | Allocated |
|--------------------------------------- |---------------:|--------------:|--------------:|---------------:|-------:|-------:|----------:|
|      GUREA01DontObserveMutableProperty | 1,275,625.5 ns | 80,176.890 ns | 236,403.37 ns | 1,180,081.9 ns | 9.7656 |      - |   72929 B |
|  GUREA02ObservableAndCriteriaMustMatch |       232.5 ns |      4.684 ns |      13.52 ns |       229.3 ns | 0.0696 |      - |     440 B |
|                  GUREA03PathMustNotify | 1,251,834.1 ns | 35,912.898 ns | 104,759.61 ns | 1,247,689.7 ns | 9.7656 |      - |   72929 B |
|                      GUREA04PreferSlim | 1,259,048.6 ns | 35,541.781 ns | 103,677.04 ns | 1,253,933.0 ns | 9.7656 |      - |   72929 B |
| GUREA05FullPathMustHaveMoreThanOneItem | 1,038,361.0 ns | 20,446.096 ns |  40,358.60 ns | 1,014,856.1 ns | 9.7656 |      - |   72929 B |
|                GUREA06DontNewCondition |   271,290.4 ns |  6,947.950 ns |  20,486.19 ns |   270,932.0 ns | 0.4883 |      - |    5412 B |
|             GUREA07DontNegateCondition | 1,104,373.0 ns | 34,887.216 ns | 102,865.74 ns | 1,061,662.1 ns | 9.7656 |      - |   72929 B |
|                GUREA08InlineSingleLine |   322,102.6 ns |  6,423.157 ns |  16,348.98 ns |   321,402.0 ns |      - |      - |     444 B |
|        GUREA09ObservableBeforeCriteria |       228.9 ns |      4.600 ns |      12.36 ns |       227.0 ns | 0.0699 |      - |     440 B |
|           GUREA10DontMergeInObservable |       246.3 ns |      5.534 ns |      16.23 ns |       243.5 ns | 0.0699 |      - |     440 B |
|       GUREA11PreferObservableFromEvent |    35,686.0 ns |  1,572.261 ns |   4,511.11 ns |    34,729.3 ns | 0.3052 | 0.0665 |    2117 B |
| GUREA12ObservableFromEventDelegateType | 1,191,656.5 ns | 33,251.613 ns |  97,521.23 ns | 1,186,671.9 ns | 9.7656 |      - |   72929 B |
|           GUREA13SyncParametersAndArgs |       233.5 ns |      4.713 ns |      12.33 ns |       234.0 ns | 0.0699 |      - |     440 B |
