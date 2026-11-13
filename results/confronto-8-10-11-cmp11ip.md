## DispatchEndToEnd | metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11
| Before | Scale=L | 35.34 ms | 43.49 ms | 14.36 ms | 0.41 | 0.33 | 2.7 MB | 1.7 MB | 0% 5%|
| After | Scale=L | 1.31 ms | 2.26 ms | 800.1 μs | 0.61 | 0.35 | 2.3 MB | 2.0 MB | 0% 5%|
| Before | Scale=M | 5.50 ms | 2.91 ms | 2.37 ms | 0.43 | 0.81 | 546.1 KB | 329.7 KB | 1% 3%|
| After | Scale=M | 253.6 μs | 154.8 μs | 154.6 μs | 0.61 | 1.00 | 435.9 KB | 389.6 KB | 1% 1%|
| Before | Scale=S | 84.8 μs | 43.2 μs | 48.5 μs | 0.57 | 1.12 | 46.3 KB | 30.4 KB | 0% 5%|
| After | Scale=S | 20.0 μs | 9.2 μs | 8.8 μs | 0.44 | 0.96 | 32.8 KB | 30.4 KB | 1% 30%|
| Before | Scale=XL | 353.22 ms | 65.05 ms | 127.29 ms | 0.36 | 1.96 | 10.3 MB | 6.5 MB | 41% 10%|
| After | Scale=XL | 7.78 ms | 4.05 ms | 3.15 ms | 0.41 | 0.78 | 9.2 MB | 8.1 MB | 2% 28%|
| Before | Scale=XXL | 1031.24 ms | 288.39 ms | 235.14 ms | 0.23 | 0.82 | 38.7 MB | 24.7 MB | 1% 46%|
| After | Scale=XXL | 41.59 ms | 11.55 ms | 11.43 ms | 0.27 | 0.99 | 35.4 MB | 31.0 MB | 1% 31%|
## ScanVsGroupBy | metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11
| RepeatedScan | ZoneCount=20,DeliveryCount=2000 | 200.2 μs | 82.8 μs | 96.1 μs | 0.48 | 1.16 | 3.0 KB | 2.1 KB | 4% 1%|
| GroupByDictionary | ZoneCount=20,DeliveryCount=2000 | 12.7 μs | 11.2 μs | 11.3 μs | 0.89 | 1.01 | 15.4 KB | 15.4 KB | 3% 1%|
| RepeatedScan | ZoneCount=20,DeliveryCount=20000 | 2.03 ms | 1.01 ms | 1.04 ms | 0.51 | 1.03 | 3.0 KB | 2.1 KB | 1% 1%|
| GroupByDictionary | ZoneCount=20,DeliveryCount=20000 | 153.3 μs | 125.4 μs | 116.0 μs | 0.76 | 0.92 | 88.4 KB | 88.5 KB | 0% 1%|
| RepeatedScan | ZoneCount=20,DeliveryCount=100000 | 10.84 ms | 6.72 ms | 6.30 ms | 0.58 | 0.94 | 3.0 KB | 2.1 KB | 1% 1%|
| GroupByDictionary | ZoneCount=20,DeliveryCount=100000 | 875.1 μs | 758.6 μs | 702.9 μs | 0.80 | 0.93 | 393.4 KB | 393.5 KB | 0% 0%|
| RepeatedScan | ZoneCount=200,DeliveryCount=2000 | 1.88 ms | 767.2 μs | 894.4 μs | 0.47 | 1.17 | 28.3 KB | 18.9 KB | 0% 1%|
| GroupByDictionary | ZoneCount=200,DeliveryCount=2000 | 24.0 μs | 17.6 μs | 17.3 μs | 0.72 | 0.98 | 40.7 KB | 39.5 KB | 1% 2%|
| RepeatedScan | ZoneCount=200,DeliveryCount=20000 | 19.25 ms | 8.64 ms | 9.53 ms | 0.50 | 1.10 | 28.4 KB | 19.0 KB | 2% 0%|
| GroupByDictionary | ZoneCount=200,DeliveryCount=20000 | 183.2 μs | 147.8 μs | 147.1 μs | 0.80 | 1.00 | 153.4 KB | 155.6 KB | 0% 0%|
| RepeatedScan | ZoneCount=200,DeliveryCount=100000 | 120.93 ms | 56.07 ms | 63.52 ms | 0.53 | 1.13 | 28.3 KB | 19.0 KB | 1% 7%|
| GroupByDictionary | ZoneCount=200,DeliveryCount=100000 | 961.4 μs | 864.3 μs | 869.7 μs | 0.90 | 1.01 | 476.2 KB | 478.4 KB | 1% 5%|
| RepeatedScan | ZoneCount=2000,DeliveryCount=2000 | 31.72 ms | 8.39 ms | 8.92 ms | 0.28 | 1.06 | 281.5 KB | 187.8 KB | 2% 0%|
| GroupByDictionary | ZoneCount=2000,DeliveryCount=2000 | 51.7 μs | 35.2 μs | 34.2 μs | 0.66 | 0.97 | 93.3 KB | 91.4 KB | 0% 0%|
| RepeatedScan | ZoneCount=2000,DeliveryCount=20000 | 378.21 ms | 82.60 ms | 93.73 ms | 0.25 | 1.13 | 282.7 KB | 188.6 KB | 2% 0%|
| GroupByDictionary | ZoneCount=2000,DeliveryCount=20000 | 332.3 μs | 273.4 μs | 343.5 μs | 1.03 | 1.26 | 403.1 KB | 399.5 KB | 14% 0%|
| RepeatedScan | ZoneCount=2000,DeliveryCount=100000 | 2077.00 ms | 986.52 ms | 1595.73 ms | 0.77 | 1.62 | 282.1 KB | 187.7 KB | 2% 2%|
| GroupByDictionary | ZoneCount=2000,DeliveryCount=100000 | 1.49 ms | 1.38 ms | 2.06 ms | 1.39 | 1.50 | 1016.9 KB | 1.0 MB | 8% 0%|
## ListVsHashSet | metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11
| ListCount | Categories=Count,VehicleCount=500,ZoneCount=20 | 13.6 μs | 5.7 μs | 6.0 μs | 0.44 | 1.06 | 0.1 KB | 0.0 KB | 2% 2%|
| HashSetCount | Categories=Count,VehicleCount=500,ZoneCount=20 | 7.2 μs | 2.7 μs | 2.7 μs | 0.37 | 1.00 | 0.1 KB | 0.0 KB | 11% 4%|
| ListCount | Categories=Count,VehicleCount=500,ZoneCount=150 | 59.2 μs | 36.1 μs | 35.4 μs | 0.60 | 0.98 | 0.1 KB | 0.0 KB | 1% 3%|
| HashSetCount | Categories=Count,VehicleCount=500,ZoneCount=150 | 5.9 μs | 2.8 μs | 2.4 μs | 0.41 | 0.85 | 0.1 KB | 0.0 KB | 3% 2%|
| ListCount | Categories=Count,VehicleCount=500,ZoneCount=700 | 260.5 μs | 171.4 μs | 167.9 μs | 0.64 | 0.98 | 0.1 KB | 0.0 KB | 1% 4%|
| HashSetCount | Categories=Count,VehicleCount=500,ZoneCount=700 | 6.9 μs | 3.4 μs | 2.3 μs | 0.33 | 0.68 | 0.1 KB | 0.0 KB | 2% 2%|
| ListCount | Categories=Count,VehicleCount=5000,ZoneCount=20 | 128.3 μs | 57.2 μs | 80.6 μs | 0.63 | 1.41 | 0.1 KB | 0.0 KB | 1% 5%|
| HashSetCount | Categories=Count,VehicleCount=5000,ZoneCount=20 | 99.3 μs | 29.6 μs | 24.1 μs | 0.24 | 0.81 | 0.1 KB | 0.0 KB | 2% 2%|
| ListCount | Categories=Count,VehicleCount=5000,ZoneCount=150 | 628.0 μs | 346.8 μs | 373.3 μs | 0.59 | 1.08 | 0.1 KB | 0.0 KB | 2% 4%|
| HashSetCount | Categories=Count,VehicleCount=5000,ZoneCount=150 | 109.2 μs | 30.2 μs | 25.9 μs | 0.24 | 0.86 | 0.1 KB | 0.0 KB | 2% 9%|
| ListCount | Categories=Count,VehicleCount=5000,ZoneCount=700 | 2.93 ms | 1.48 ms | 1.65 ms | 0.56 | 1.11 | 0.1 KB | 0.0 KB | 1% 9%|
| HashSetCount | Categories=Count,VehicleCount=5000,ZoneCount=700 | 101.6 μs | 30.9 μs | 28.5 μs | 0.28 | 0.92 | 0.1 KB | 0.0 KB | 11% 3%|
| ListContains | Categories=ToList,VehicleCount=500,ZoneCount=20 | 12.0 μs | 6.1 μs | 7.4 μs | 0.62 | 1.22 | 4.3 KB | 1.3 KB | 1% 3%|
| HashSetContains | Categories=ToList,VehicleCount=500,ZoneCount=20 | 4.5 μs | 3.3 μs | 2.9 μs | 0.66 | 0.90 | 4.3 KB | 1.3 KB | 2% 2%|
| FrozenSetContains | Categories=ToList,VehicleCount=500,ZoneCount=20 | 5.9 μs | 4.0 μs | 4.1 μs | 0.69 | 1.02 | 4.3 KB | 1.3 KB | 2% 3%|
| ListContains | Categories=ToList,VehicleCount=500,ZoneCount=150 | 75.6 μs | 34.2 μs | 46.0 μs | 0.61 | 1.35 | 4.3 KB | 1.3 KB | 1% 12%|
| HashSetContains | Categories=ToList,VehicleCount=500,ZoneCount=150 | 4.6 μs | 3.7 μs | 3.4 μs | 0.73 | 0.91 | 4.3 KB | 1.3 KB | 14% 3%|
| FrozenSetContains | Categories=ToList,VehicleCount=500,ZoneCount=150 | 6.0 μs | 3.9 μs | 4.1 μs | 0.69 | 1.05 | 4.3 KB | 1.3 KB | 16% 2%|
| ListContains | Categories=ToList,VehicleCount=500,ZoneCount=700 | 262.5 μs | 157.7 μs | 194.1 μs | 0.74 | 1.23 | 4.3 KB | 1.3 KB | 9% 3%|
| HashSetContains | Categories=ToList,VehicleCount=500,ZoneCount=700 | 4.4 μs | 3.5 μs | 2.8 μs | 0.64 | 0.80 | 4.3 KB | 1.3 KB | 2% 1%|
| FrozenSetContains | Categories=ToList,VehicleCount=500,ZoneCount=700 | 10.1 μs | 4.4 μs | 4.0 μs | 0.40 | 0.92 | 4.3 KB | 1.3 KB | 10% 33%|
| ListContains | Categories=ToList,VehicleCount=5000,ZoneCount=20 | 118.8 μs | 67.0 μs | 73.9 μs | 0.62 | 1.10 | 32.4 KB | 13.3 KB | 2% 2%|
| HashSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=20 | 81.0 μs | 41.4 μs | 33.8 μs | 0.42 | 0.82 | 32.4 KB | 13.3 KB | 5% 15%|
| FrozenSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=20 | 67.0 μs | 57.8 μs | 52.8 μs | 0.79 | 0.91 | 32.4 KB | 13.3 KB | 1% 2%|
| ListContains | Categories=ToList,VehicleCount=5000,ZoneCount=150 | 578.7 μs | 426.6 μs | 405.8 μs | 0.70 | 0.95 | 32.4 KB | 12.4 KB | 4% 3%|
| HashSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=150 | 58.0 μs | 40.5 μs | 31.1 μs | 0.54 | 0.77 | 32.4 KB | 12.4 KB | 5% 4%|
| FrozenSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=150 | 65.9 μs | 57.6 μs | 53.8 μs | 0.82 | 0.93 | 32.4 KB | 12.4 KB | 3% 1%|
| ListContains | Categories=ToList,VehicleCount=5000,ZoneCount=700 | 2.58 ms | 1.69 ms | 1.87 ms | 0.73 | 1.11 | 32.4 KB | 12.4 KB | 4% 4%|
| HashSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=700 | 63.9 μs | 40.6 μs | 32.8 μs | 0.51 | 0.81 | 32.4 KB | 12.4 KB | 6% 2%|
| FrozenSetContains | Categories=ToList,VehicleCount=5000,ZoneCount=700 | 69.0 μs | 58.0 μs | 53.0 μs | 0.77 | 0.91 | 32.4 KB | 12.4 KB | 3% 3%|
## AllocationsVsForeach | metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11
| CountGreaterZero | Categories=Exists,Scale=L | 125.6 μs | 27.2 μs | 13.8 μs | 0.11 | 0.51 | 146.7 KB | 0.1 KB | 1% 3%|
| AnyEarlyExit | Categories=Exists,Scale=L | 31 ns | 4 ns | 3 ns | 0.10 | 0.74 | 0.1 KB | 0.0 KB | 2% 3%|
| CountGreaterZero | Categories=Exists,Scale=M | 49.3 μs | 8.4 μs | 5.0 μs | 0.10 | 0.59 | 58.9 KB | 0.1 KB | 1% 5%|
| AnyEarlyExit | Categories=Exists,Scale=M | 153 ns | 29 ns | 22 ns | 0.15 | 0.77 | 0.2 KB | 0.0 KB | 1% 3%|
| CountGreaterZero | Categories=Exists,Scale=S | 3.8 μs | 824 ns | 449 ns | 0.12 | 0.54 | 6.2 KB | 0.1 KB | 1% 4%|
| AnyEarlyExit | Categories=Exists,Scale=S | 90 ns | 18 ns | 14 ns | 0.16 | 0.76 | 0.2 KB | 0.0 KB | 1% 2%|
| CountGreaterZero | Categories=Exists,Scale=XL | 255.9 μs | 46.1 μs | 31.3 μs | 0.12 | 0.68 | 293.8 KB | 0.1 KB | 0% 3%|
| AnyEarlyExit | Categories=Exists,Scale=XL | 37 ns | 6 ns | 5 ns | 0.13 | 0.77 | 0.1 KB | 0.0 KB | 2% 3%|
| OrderByFirst | Categories=Max,Scale=L | 28.7 μs | 16.7 μs | 12.1 μs | 0.42 | 0.73 | 0.2 KB | 0.2 KB | 1% 6%|
| MaxByLinear | Categories=Max,Scale=L | 33.3 μs | 26.4 μs | 20.2 μs | 0.61 | 0.76 | 0.1 KB | 0.1 KB | 1% 6%|
| OrderByFirst | Categories=Max,Scale=M | 10.5 μs | 6.2 μs | 4.7 μs | 0.45 | 0.76 | 0.2 KB | 0.2 KB | 1% 4%|
| MaxByLinear | Categories=Max,Scale=M | 12.9 μs | 8.0 μs | 7.9 μs | 0.61 | 0.99 | 0.1 KB | 0.1 KB | 3% 9%|
| OrderByFirst | Categories=Max,Scale=S | 799 ns | 420 ns | 423 ns | 0.53 | 1.01 | 0.2 KB | 0.2 KB | 2% 8%|
| MaxByLinear | Categories=Max,Scale=S | 1.0 μs | 625 ns | 604 ns | 0.58 | 0.97 | 0.1 KB | 0.1 KB | 1% 3%|
| OrderByFirst | Categories=Max,Scale=XL | 56.0 μs | 27.5 μs | 26.6 μs | 0.47 | 0.97 | 0.2 KB | 0.2 KB | 2% 1%|
| MaxByLinear | Categories=Max,Scale=XL | 67.4 μs | 42.7 μs | 43.2 μs | 0.64 | 1.01 | 0.1 KB | 0.1 KB | 4% 8%|
| IntermediateToList | Categories=Pipeline,Scale=L | 60.5 μs | 34.3 μs | 28.4 μs | 0.47 | 0.83 | 102.2 KB | 59.1 KB | 1% 3%|
| LazyPipeline | Categories=Pipeline,Scale=L | 46.5 μs | 28.7 μs | 27.0 μs | 0.58 | 0.94 | 30.2 KB | 21.8 KB | 3% 5%|
| IntermediateToList | Categories=Pipeline,Scale=M | 25.1 μs | 11.5 μs | 10.3 μs | 0.41 | 0.90 | 49.1 KB | 23.7 KB | 1% 12%|
| LazyPipeline | Categories=Pipeline,Scale=M | 15.9 μs | 9.7 μs | 9.6 μs | 0.60 | 0.98 | 13.2 KB | 8.8 KB | 3% 2%|
| IntermediateToList | Categories=Pipeline,Scale=S | 3.3 μs | 1.1 μs | 955 ns | 0.29 | 0.90 | 6.1 KB | 2.9 KB | 1% 31%|
| LazyPipeline | Categories=Pipeline,Scale=S | 2.9 μs | 851 ns | 820 ns | 0.28 | 0.96 | 1.8 KB | 1.4 KB | 2% 15%|
| IntermediateToList | Categories=Pipeline,Scale=XL | 363.3 μs | 71.9 μs | 62.4 μs | 0.17 | 0.87 | 204.5 KB | 119.5 KB | 3% 11%|
| LazyPipeline | Categories=Pipeline,Scale=XL | 253.1 μs | 64.5 μs | 57.6 μs | 0.23 | 0.89 | 60.5 KB | 44.1 KB | 1% 15%|
## TripleNesting | metodo | param | net8 | net10 | net11 | 11/8 | 11/10 | alloc 8 | alloc 11 | sd%11
| Nested | Scale=L | 28.87 ms | 14.10 ms | 12.81 ms | 0.44 | 0.91 | 2.4 MB | 1.6 MB | 2% 1%|
| Lookup | Scale=L | 1.24 ms | 802.6 μs | 809.9 μs | 0.65 | 1.01 | 2.3 MB | 2.0 MB | 1% 3%|
| Nested | Scale=M | 4.64 ms | 2.18 ms | 2.00 ms | 0.43 | 0.92 | 439.6 KB | 306.7 KB | 1% 1%|
| Lookup | Scale=M | 240.9 μs | 154.6 μs | 152.8 μs | 0.63 | 0.99 | 433.6 KB | 387.8 KB | 0% 0%|
| Nested | Scale=S | 44.2 μs | 28.5 μs | 25.2 μs | 0.57 | 0.88 | 29.4 KB | 24.7 KB | 0% 1%|
| Lookup | Scale=S | 11.5 μs | 8.2 μs | 8.3 μs | 0.72 | 1.02 | 30.6 KB | 28.6 KB | 1% 0%|
| Nested | Scale=XL | 116.72 ms | 71.21 ms | 51.15 ms | 0.44 | 0.72 | 9.8 MB | 6.4 MB | 1% 1%|
| Lookup | Scale=XL | 4.35 ms | 2.99 ms | 3.17 ms | 0.73 | 1.06 | 9.2 MB | 8.1 MB | 3% 1%|
| Nested | Scale=XXL | 449.43 ms | 232.75 ms | 195.92 ms | 0.44 | 0.84 | 37.5 MB | 24.5 MB | 1% 1%|
| Lookup | Scale=XXL | 16.14 ms | 11.70 ms | 11.60 ms | 0.72 | 0.99 | 35.4 MB | 31.0 MB | 1% 2%|
## ZoneCoverage
| Classic | Zones=20,Scale=L | 44.8 μs ±627 ns | 46.7 KB |
| Modern | Zones=20,Scale=L | 37.6 μs ±302 ns | 10.1 KB |
| Classic | Zones=20,Scale=XXL | 178.7 μs ±1.2 μs | 137.3 KB |
| Modern | Zones=20,Scale=XXL | 154.1 μs ±1.5 μs | 10.1 KB |
| Classic | Zones=2000,Scale=L | 488.0 μs ±1.7 μs | 362.5 KB |
| Modern | Zones=2000,Scale=L | 124.8 μs ±549 ns | 173.7 KB |
| Classic | Zones=2000,Scale=XXL | 2.07 ms ±13.4 μs | 995.7 KB |
| Modern | Zones=2000,Scale=XXL | 410.1 μs ±1.7 μs | 469.6 KB |
