```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat) (container)
-
.NET SDK 10.0.403
  [Host]     : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.14 (10.0.14, 10.0.1426.52326), Arm64 RyuJIT armv8.0-a


```
| Method            | Scale | Mean        | Error       | StdDev      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------ |------ |------------:|------------:|------------:|------:|--------:|-------:|----------:|------------:|
| **ThreeEnumerations** | **L**     | **26,838.0 ns** |   **287.04 ns** |   **239.69 ns** |  **1.00** |    **0.01** |      **-** |      **72 B** |        **1.00** |
| MaterializedOnce  | L     | 12,443.7 ns |   136.99 ns |   114.39 ns |  0.46 |    0.01 | 0.6409 |    2736 B |       38.00 |
|                   |       |             |             |             |       |         |        |           |             |
| **ThreeEnumerations** | **M**     |  **9,914.8 ns** |   **191.87 ns** |   **149.80 ns** |  **1.00** |    **0.02** | **0.0153** |      **72 B** |        **1.00** |
| MaterializedOnce  | M     |  4,689.9 ns |    93.37 ns |   114.67 ns |  0.47 |    0.01 | 0.2747 |    1168 B |       16.22 |
|                   |       |             |             |             |       |         |        |           |             |
| **ThreeEnumerations** | **S**     |    **900.1 ns** |     **8.42 ns** |     **7.03 ns** |  **1.00** |    **0.01** | **0.0172** |      **72 B** |        **1.00** |
| MaterializedOnce  | S     |    474.4 ns |     8.89 ns |     8.73 ns |  0.53 |    0.01 | 0.0668 |     280 B |        3.89 |
|                   |       |             |             |             |       |         |        |           |             |
| **ThreeEnumerations** | **XL**    | **58,621.0 ns** | **1,086.82 ns** | **1,016.61 ns** |  **1.00** |    **0.02** |      **-** |      **72 B** |        **1.00** |
| MaterializedOnce  | XL    | 27,019.1 ns |   439.21 ns |   366.76 ns |  0.46 |    0.01 | 1.2512 |    5328 B |       74.00 |
