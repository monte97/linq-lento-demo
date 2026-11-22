
## AllocationsVsForeach

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| AnyEarlyExit | Scale=L | 0.1 μs / 0.1 μs | 0.0 μs / 0.0 μs | 0.0 μs / 0.0 μs | 0.0 μs / 0.0 μs | 0 B |
| AnyEarlyExit | Scale=M | 0.3 μs / 0.3 μs | 0.2 μs / 0.2 μs | 0.1 μs / 0.0 μs | 0.1 μs / 0.0 μs | 0 B |
| AnyEarlyExit | Scale=S | 0.2 μs / 0.2 μs | 0.1 μs / 0.1 μs | 0.0 μs / 0.0 μs | 0.0 μs / 0.0 μs | 0 B |
| AnyEarlyExit | Scale=XL | 0.1 μs / 0.1 μs | 0.1 μs / 0.1 μs | 0.0 μs / 0.0 μs | 0.0 μs / 0.0 μs | 0 B |
| CountGreaterZero | Scale=L | 281.4 μs / 208.2 μs | 163.0 μs / 111.9 μs | 27.0 μs / 22.2 μs | 25.6 μs / 21.2 μs | 72 B |
| CountGreaterZero | Scale=M | 109.9 μs / 81.3 μs | 52.7 μs / 42.5 μs | 10.3 μs / 7.7 μs | 9.6 μs / 7.4 μs | 72 B |
| CountGreaterZero | Scale=S | 10.5 μs / 8.1 μs | 4.9 μs / 4.3 μs | 1.0 μs / 0.8 μs | 0.9 μs / 0.8 μs | 72 B |
| CountGreaterZero | Scale=XL | 556.5 μs / 478.6 μs | 324.0 μs / 241.0 μs | 101.0 μs / 43.1 μs | 53.9 μs / 41.2 μs | 72 B |
| IntermediateToList | Scale=L | 266.9 μs / 204.6 μs | 88.3 μs / 69.2 μs | 94.6 μs / 60.4 μs | 94.8 μs / 50.0 μs | 60480 B |
| IntermediateToList | Scale=M | 98.7 μs / 76.2 μs | 30.1 μs / 27.1 μs | 29.7 μs / 22.4 μs | 29.7 μs / 19.4 μs | 24272 B |
| IntermediateToList | Scale=S | 8.9 μs / 7.2 μs | 2.6 μs / 2.8 μs | 2.8 μs / 2.3 μs | 2.5 μs / 2.3 μs | 3000 B |
| IntermediateToList | Scale=XL | 614.8 μs / 451.6 μs | 234.0 μs / 141.1 μs | 250.4 μs / 134.6 μs | 244.5 μs / 105.9 μs | 122329 B |
| LazyPipeline | Scale=L | 184.9 μs / 122.2 μs | 64.1 μs / 52.1 μs | 71.0 μs / 53.3 μs | 66.3 μs / 42.4 μs | 22321 B |
| LazyPipeline | Scale=M | 57.4 μs / 43.2 μs | 22.9 μs / 20.1 μs | 22.7 μs / 18.7 μs | 22.2 μs / 16.1 μs | 9048 B |
| LazyPipeline | Scale=S | 4.7 μs / 3.8 μs | 2.2 μs / 1.9 μs | 2.1 μs / 1.8 μs | 2.0 μs / 1.6 μs | 1456 B |
| LazyPipeline | Scale=XL | 437.9 μs / 283.7 μs | 211.3 μs / 106.0 μs | 227.4 μs / 111.4 μs | 218.4 μs / 86.2 μs | 45193 B |
| MaxByLinear | Scale=L | - / - | 35.1 μs / 24.1 μs | 39.0 μs / 24.9 μs | 36.0 μs / 26.2 μs | 72 B |
| MaxByLinear | Scale=M | - / - | 14.1 μs / 10.6 μs | 13.4 μs / 9.8 μs | 13.0 μs / 10.4 μs | 72 B |
| MaxByLinear | Scale=S | - / - | 1.3 μs / 1.1 μs | 1.3 μs / 1.1 μs | 1.3 μs / 1.1 μs | 72 B |
| MaxByLinear | Scale=XL | - / - | 83.3 μs / 52.3 μs | 88.8 μs / 53.3 μs | 83.1 μs / 53.4 μs | 73 B |
| OrderByFirst | Scale=L | 186.7 μs / 126.6 μs | 33.0 μs / 18.9 μs | 34.7 μs / 18.3 μs | 32.5 μs / 21.7 μs | 160 B |
| OrderByFirst | Scale=M | 57.6 μs / 45.5 μs | 9.3 μs / 7.5 μs | 9.5 μs / 7.4 μs | 9.1 μs / 7.2 μs | 160 B |
| OrderByFirst | Scale=S | 4.6 μs / 3.9 μs | 0.9 μs / 0.9 μs | 1.0 μs / 0.8 μs | 0.9 μs / 0.8 μs | 160 B |
| OrderByFirst | Scale=XL | 449.2 μs / 302.3 μs | 78.7 μs / 43.3 μs | 75.1 μs / 44.1 μs | 72.5 μs / 45.5 μs | 161 B |

## DispatchEndToEnd

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| After | Scale=L | 2.7 ms / 2.0 ms | 2.4 ms / 1.8 ms | 1.8 ms / 1.5 ms | 1.8 ms / 1.4 ms | 2084.97 KB |
| After | Scale=M | 568.7 μs / 417.4 μs | 492.6 μs / 354.2 μs | 345.1 μs / 277.2 μs | 348.0 μs / 273.3 μs | 389.58 KB |
| After | Scale=S | 41.6 μs / 26.5 μs | 27.1 μs / 20.8 μs | 20.5 μs / 18.7 μs | 19.7 μs / 17.9 μs | 30.38 KB |
| After | Scale=XL | 9.5 ms / 8.0 ms | 9.1 ms / 7.2 ms | 6.8 ms / 5.6 ms | 6.7 ms / 5.5 ms | 8278.79 KB |
| After | Scale=XXL | 34.5 ms / 29.4 ms | 34.1 ms / 28.1 ms | 25.8 ms / 21.8 ms | 25.4 ms / 22.3 ms | 31744.48 KB |
| Before | Scale=L | 129.6 ms / 99.4 ms | 70.3 ms / 51.2 ms | 38.2 ms / 28.4 ms | 37.6 ms / 27.6 ms | 1718.94 KB |
| Before | Scale=M | 21.6 ms / 16.8 ms | 11.3 ms / 8.7 ms | 5.7 ms / 5.0 ms | 5.7 ms / 4.3 ms | 329.68 KB |
| Before | Scale=S | 323.3 μs / 252.2 μs | 167.0 μs / 135.3 μs | 99.5 μs / 81.1 μs | 98.1 μs / 78.8 μs | 30.38 KB |
| Before | Scale=XL | 543.4 ms / 379.3 ms | 284.0 ms / 228.4 ms | 155.6 ms / 124.9 ms | 150.6 ms / 123.3 ms | 6677.85 KB |
| Before | Scale=XXL | 1.83 s / 1.46 s | 1.07 s / 869.0 ms | 602.1 ms / 491.5 ms | 578.6 ms / 470.0 ms | 25294.24 KB |

## ListVsHashSet

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| FrozenSetContains | VehicleCount=500 ZoneCount=150 | 21.3 μs / 15.2 μs | 6.7 μs / 5.7 μs | 6.5 μs / 5.0 μs | 6.0 μs / 5.0 μs | 1336 B |
| FrozenSetContains | VehicleCount=500 ZoneCount=20 | 20.9 μs / 14.6 μs | 6.5 μs / 5.2 μs | 6.5 μs / 5.0 μs | 5.8 μs / 4.9 μs | 1376 B |
| FrozenSetContains | VehicleCount=500 ZoneCount=700 | 21.8 μs / 16.0 μs | 7.1 μs / 5.9 μs | 6.8 μs / 5.2 μs | 6.3 μs / 5.2 μs | 1344 B |
| FrozenSetContains | VehicleCount=5000 ZoneCount=150 | 231.4 μs / 153.4 μs | 78.4 μs / 57.5 μs | 92.4 μs / 48.7 μs | 83.5 μs / 48.7 μs | 12649 B |
| FrozenSetContains | VehicleCount=5000 ZoneCount=20 | 230.2 μs / 148.5 μs | 79.9 μs / 53.4 μs | 93.5 μs / 48.5 μs | 81.4 μs / 48.3 μs | 13601 B |
| FrozenSetContains | VehicleCount=5000 ZoneCount=700 | 249.7 μs / 162.6 μs | 97.3 μs / 62.2 μs | 97.7 μs / 52.1 μs | 89.5 μs / 50.9 μs | 12673 B |
| HashSetContains | VehicleCount=500 ZoneCount=150 | 13.6 μs / 10.9 μs | 7.6 μs / 6.5 μs | 6.7 μs / 5.5 μs | 5.4 μs / 4.5 μs | 1336 B |
| HashSetContains | VehicleCount=500 ZoneCount=20 | 13.9 μs / 10.8 μs | 7.3 μs / 6.3 μs | 6.6 μs / 5.4 μs | 5.2 μs / 4.3 μs | 1376 B |
| HashSetContains | VehicleCount=500 ZoneCount=700 | 14.1 μs / 11.0 μs | 7.7 μs / 6.7 μs | 6.7 μs / 5.6 μs | 5.4 μs / 4.6 μs | 1344 B |
| HashSetContains | VehicleCount=5000 ZoneCount=150 | 204.3 μs / 113.8 μs | 91.3 μs / 68.4 μs | 100.6 μs / 53.8 μs | 79.4 μs / 44.6 μs | 12649 B |
| HashSetContains | VehicleCount=5000 ZoneCount=20 | 201.2 μs / 114.1 μs | 92.1 μs / 65.8 μs | 98.6 μs / 53.2 μs | 77.8 μs / 43.2 μs | 13601 B |
| HashSetContains | VehicleCount=5000 ZoneCount=700 | 227.6 μs / 117.4 μs | 97.8 μs / 69.6 μs | 98.2 μs / 55.0 μs | 79.4 μs / 44.7 μs | 12672 B |
| HashSetCount | VehicleCount=500 ZoneCount=150 | 13.7 μs / 10.8 μs | 8.2 μs / 6.9 μs | 5.5 μs / 4.4 μs | 4.3 μs / 3.5 μs | 0 B |
| HashSetCount | VehicleCount=500 ZoneCount=20 | 13.4 μs / 10.4 μs | 7.6 μs / 6.4 μs | 5.4 μs / 4.3 μs | 4.1 μs / 3.5 μs | 0 B |
| HashSetCount | VehicleCount=500 ZoneCount=700 | 14.0 μs / 10.9 μs | 8.3 μs / 6.9 μs | 5.5 μs / 4.4 μs | 4.6 μs / 3.6 μs | 0 B |
| HashSetCount | VehicleCount=5000 ZoneCount=150 | 194.0 μs / 108.9 μs | 101.2 μs / 72.2 μs | 58.8 μs / 46.3 μs | 46.9 μs / 36.4 μs | 0 B |
| HashSetCount | VehicleCount=5000 ZoneCount=20 | 197.1 μs / 108.5 μs | 110.4 μs / 70.3 μs | 58.7 μs / 44.5 μs | 43.6 μs / 37.1 μs | 0 B |
| HashSetCount | VehicleCount=5000 ZoneCount=700 | 204.3 μs / 110.4 μs | 102.7 μs / 71.8 μs | 65.3 μs / 44.4 μs | 58.2 μs / 35.9 μs | 0 B |
| ListContains | VehicleCount=500 ZoneCount=150 | 175.1 μs / 130.0 μs | 107.7 μs / 85.4 μs | 88.2 μs / 68.8 μs | 88.3 μs / 69.6 μs | 1337 B |
| ListContains | VehicleCount=500 ZoneCount=20 | 36.0 μs / 26.4 μs | 20.1 μs / 16.3 μs | 14.6 μs / 11.3 μs | 14.4 μs / 11.2 μs | 1376 B |
| ListContains | VehicleCount=500 ZoneCount=700 | 765.4 μs / 579.0 μs | 477.9 μs / 394.5 μs | 397.0 μs / 320.2 μs | 397.8 μs / 322.4 μs | 1347 B |
| ListContains | VehicleCount=5000 ZoneCount=150 | 1.7 ms / 1.3 ms | 1.1 ms / 862.1 μs | 871.0 μs / 690.6 μs | 875.1 μs / 696.0 μs | 12654 B |
| ListContains | VehicleCount=5000 ZoneCount=20 | 357.5 μs / 268.4 μs | 203.9 μs / 162.2 μs | 151.7 μs / 108.8 μs | 151.7 μs / 108.8 μs | 13601 B |
| ListContains | VehicleCount=5000 ZoneCount=700 | 7.4 ms / 5.8 ms | 4.7 ms / 3.9 ms | 3.9 ms / 3.2 ms | 4.0 ms / 3.2 ms | 12716 B |
| ListCount | VehicleCount=500 ZoneCount=150 | 173.9 μs / 130.2 μs | 109.3 μs / 91.3 μs | 80.4 μs / 68.0 μs | 79.5 μs / 67.4 μs | 0 B |
| ListCount | VehicleCount=500 ZoneCount=20 | 33.9 μs / 25.9 μs | 21.9 μs / 17.0 μs | 12.6 μs / 10.1 μs | 12.6 μs / 10.1 μs | 0 B |
| ListCount | VehicleCount=500 ZoneCount=700 | 749.3 μs / 576.3 μs | 477.8 μs / 394.1 μs | 365.2 μs / 318.8 μs | 364.6 μs / 318.2 μs | 0 B |
| ListCount | VehicleCount=5000 ZoneCount=150 | 1.7 ms / 1.3 ms | 1.1 ms / 879.5 μs | 799.4 μs / 674.1 μs | 794.0 μs / 678.0 μs | 0 B |
| ListCount | VehicleCount=5000 ZoneCount=20 | 340.5 μs / 260.9 μs | 215.4 μs / 169.9 μs | 132.4 μs / 99.5 μs | 130.1 μs / 99.3 μs | 0 B |
| ListCount | VehicleCount=5000 ZoneCount=700 | 7.4 ms / 5.8 ms | 4.7 ms / 4.0 ms | 3.6 ms / 3.2 ms | 3.6 ms / 3.2 ms | 0 B |

## ScanVsGroupBy

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| GroupByDictionary | ZoneCount=20 DeliveryCount=100000 | - / - | 1.7 ms / 1.8 ms | 1.6 ms / 1.6 ms | 1.5 ms / 1.5 ms | 393.49 KB |
| GroupByDictionary | ZoneCount=20 DeliveryCount=2000 | - / - | 21.8 μs / 17.6 μs | 24.5 μs / 22.2 μs | 22.9 μs / 21.8 μs | 15.41 KB |
| GroupByDictionary | ZoneCount=20 DeliveryCount=20000 | - / - | 295.9 μs / 208.1 μs | 288.2 μs / 249.4 μs | 274.0 μs / 237.1 μs | 88.46 KB |
| GroupByDictionary | ZoneCount=200 DeliveryCount=100000 | - / - | 1.9 ms / 2.4 ms | 1.9 ms / 2.0 ms | 1.9 ms / 1.8 ms | 478.42 KB |
| GroupByDictionary | ZoneCount=200 DeliveryCount=2000 | - / - | 38.7 μs / 30.3 μs | 41.9 μs / 38.3 μs | 39.3 μs / 36.0 μs | 39.51 KB |
| GroupByDictionary | ZoneCount=200 DeliveryCount=20000 | - / - | 374.2 μs / 279.3 μs | 386.2 μs / 305.6 μs | 361.6 μs / 300.4 μs | 155.62 KB |
| GroupByDictionary | ZoneCount=2000 DeliveryCount=100000 | - / - | 3.1 ms / 2.5 ms | 3.3 ms / 3.8 ms | 3.2 ms / 3.2 ms | 1044.96 KB |
| GroupByDictionary | ZoneCount=2000 DeliveryCount=2000 | - / - | 81.4 μs / 62.1 μs | 83.1 μs / 73.7 μs | 78.6 μs / 65.8 μs | 91.42 KB |
| GroupByDictionary | ZoneCount=2000 DeliveryCount=20000 | - / - | 650.2 μs / 494.8 μs | 684.6 μs / 585.5 μs | 666.7 μs / 566.3 μs | 399.49 KB |
| RepeatedScan | ZoneCount=20 DeliveryCount=100000 | 35.3 ms / 35.3 ms | 24.4 ms / 26.8 ms | 14.5 ms / 14.0 ms | 14.6 ms / 15.1 ms | 2.14 KB |
| RepeatedScan | ZoneCount=20 DeliveryCount=2000 | 609.2 μs / 508.3 μs | 393.8 μs / 309.9 μs | 205.9 μs / 152.1 μs | 206.3 μs / 152.7 μs | 2.06 KB |
| RepeatedScan | ZoneCount=20 DeliveryCount=20000 | 6.6 ms / 5.7 ms | 4.6 ms / 3.7 ms | 2.7 ms / 2.3 ms | 2.7 ms / 2.4 ms | 2.08 KB |
| RepeatedScan | ZoneCount=200 DeliveryCount=100000 | 496.6 ms / 467.2 ms | 233.1 ms / 208.9 ms | 134.5 ms / 126.6 ms | 135.5 ms / 127.1 ms | 20.24 KB |
| RepeatedScan | ZoneCount=200 DeliveryCount=2000 | 8.0 ms / 5.3 ms | 3.7 ms / 3.0 ms | 1.9 ms / 1.6 ms | 1.9 ms / 1.6 ms | 18.95 KB |
| RepeatedScan | ZoneCount=200 DeliveryCount=20000 | 94.2 ms / 83.5 ms | 43.7 ms / 33.9 ms | 24.8 ms / 22.1 ms | 24.6 ms / 22.4 ms | 19.09 KB |
| RepeatedScan | ZoneCount=2000 DeliveryCount=100000 | 5.34 s / 4.77 s | 3.86 s / 3.46 s | 2.30 s / 1.96 s | 2.26 s / 1.97 s | 192.61 KB |
| RepeatedScan | ZoneCount=2000 DeliveryCount=2000 | 82.2 ms / 48.4 ms | 46.3 ms / 27.9 ms | 15.0 ms / 11.7 ms | 15.5 ms / 11.6 ms | 187.85 KB |
| RepeatedScan | ZoneCount=2000 DeliveryCount=20000 | 1.04 s / 796.9 ms | 746.8 ms / 537.2 ms | 417.9 ms / 175.9 ms | 406.0 ms / 175.7 ms | 192.89 KB |

## TripleNesting

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| Lookup | Scale=L | 2.4 ms / 2.0 ms | 2.1 ms / 1.7 ms | 1.7 ms / 1.4 ms | 1.7 ms / 1.4 ms | 2083.18 KB |
| Lookup | Scale=M | 504.1 μs / 390.3 μs | 429.2 μs / 323.9 μs | 337.4 μs / 280.7 μs | 331.0 μs / 277.7 μs | 387.79 KB |
| Lookup | Scale=S | 31.2 μs / 21.8 μs | 22.7 μs / 17.9 μs | 18.6 μs / 17.9 μs | 18.7 μs / 17.9 μs | 28.65 KB |
| Lookup | Scale=XL | 9.0 ms / 7.5 ms | 7.9 ms / 6.8 ms | 6.8 ms / 5.7 ms | 6.7 ms / 5.8 ms | 8276.99 KB |
| Lookup | Scale=XXL | 33.6 ms / 27.4 ms | 29.6 ms / 25.8 ms | 26.0 ms / 22.4 ms | 25.7 ms / 22.7 ms | 31742.65 KB |
| Nested | Scale=L | 113.3 ms / 90.1 ms | 67.2 ms / 47.2 ms | 35.7 ms / 24.9 ms | 31.8 ms / 22.7 ms | 1667.79 KB |
| Nested | Scale=M | 19.2 ms / 14.5 ms | 10.5 ms / 7.7 ms | 5.2 ms / 3.9 ms | 4.8 ms / 3.6 ms | 306.7 KB |
| Nested | Scale=S | 208.8 μs / 156.6 μs | 102.3 μs / 74.7 μs | 59.7 μs / 48.1 μs | 55.6 μs / 43.7 μs | 24.72 KB |
| Nested | Scale=XL | 485.8 ms / 383.1 ms | 281.3 ms / 203.8 ms | 144.8 ms / 107.5 ms | 131.8 ms / 98.4 ms | 6578.03 KB |
| Nested | Scale=XXL | 1.86 s / 1.44 s | 1.05 s / 819.1 ms | 566.4 ms / 423.8 ms | 512.4 ms / 384.4 ms | 25097.47 KB |

## ZoneCoverage

| Metodo | Parametri | net48 g1 / g2 | net8 g1 / g2 | net10 g1 / g2 | net11 g1 / g2 | alloc 11 |
|---|---|---|---|---|---|---|
| Classic | Zones=20 Scale=L | - / - | - / - | - / - | 86.7 μs / 74.2 μs | 46.67 KB |
| Classic | Zones=20 Scale=XXL | - / - | - / - | - / - | 358.1 μs / 305.6 μs | 137.34 KB |
| Classic | Zones=2000 Scale=L | - / - | - / - | - / - | 1.1 ms / 934.3 μs | 362.52 KB |
| Classic | Zones=2000 Scale=XXL | - / - | - / - | - / - | 4.5 ms / 3.8 ms | 995.77 KB |
| ClassicHashSet | Zones=20 Scale=L | - / - | - / - | - / - | 93.5 μs / 85.1 μs | 46.7 KB |
| ClassicHashSet | Zones=20 Scale=XXL | - / - | - / - | - / - | 377.1 μs / 346.3 μs | 137.36 KB |
| ClassicHashSet | Zones=2000 Scale=L | - / - | - / - | - / - | 486.6 μs / 412.0 μs | 369.61 KB |
| ClassicHashSet | Zones=2000 Scale=XXL | - / - | - / - | - / - | 1.5 ms / 1.4 ms | 986.78 KB |
| Modern | Zones=20 Scale=L | - / - | - / - | - / - | 78.5 μs / 48.0 μs | 10.14 KB |
| Modern | Zones=20 Scale=XXL | - / - | - / - | - / - | 319.6 μs / 196.2 μs | 10.14 KB |
| Modern | Zones=2000 Scale=L | - / - | - / - | - / - | 359.6 μs / 256.8 μs | 173.66 KB |
| Modern | Zones=2000 Scale=XXL | - / - | - / - | - / - | 1.1 ms / 979.9 μs | 469.66 KB |
