namespace Cistern.Linq.Benchmarking.FSharp.ChunkBySize

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

type ChunkBySizeBase () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    [<Params(1, 5, 10)>]
    member val ChunkSize = 0 with get, set

    member val Source = [||] with get, set

    [<GlobalSetup>]
    member this.Setup() =
        this.Source <- Array.init this.NumberOfItems (fun x -> float (x + 1))

(*
|  Method | NumberOfItems | ChunkSize |         Mean |       Error |      StdDev |       Median | Ratio | RatioSD |
|-------- |-------------- |---------- |-------------:|------------:|------------:|-------------:|------:|--------:|
|     Seq |             0 |         1 |     56.94 ns |   0.9798 ns |   0.8685 ns |     57.17 ns |  1.00 |    0.00 |
| Cistern |             0 |         1 |     53.55 ns |   0.6085 ns |   0.5395 ns |     53.70 ns |  0.94 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |             0 |         5 |     56.16 ns |   0.7919 ns |   0.7020 ns |     55.74 ns |  1.00 |    0.00 |
| Cistern |             0 |         5 |     51.01 ns |   0.7453 ns |   0.6972 ns |     51.49 ns |  0.91 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |             0 |        10 |     56.91 ns |   0.7912 ns |   0.7401 ns |     57.31 ns |  1.00 |    0.00 |
| Cistern |             0 |        10 |     53.99 ns |   0.8552 ns |   0.8000 ns |     54.24 ns |  0.95 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |             1 |         1 |     86.43 ns |   1.1226 ns |   1.0501 ns |     86.06 ns |  1.00 |    0.00 |
| Cistern |             1 |         1 |     97.76 ns |   1.3823 ns |   1.2930 ns |     97.91 ns |  1.13 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |             1 |         5 |     98.41 ns |   0.7418 ns |   0.6938 ns |     98.55 ns |  1.00 |    0.00 |
| Cistern |             1 |         5 |    125.02 ns |   1.9749 ns |   1.8473 ns |    125.49 ns |  1.27 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |             1 |        10 |    112.11 ns |   1.7978 ns |   1.4036 ns |    112.19 ns |  1.00 |    0.00 |
| Cistern |             1 |        10 |    135.89 ns |   2.7078 ns |   3.4245 ns |    135.82 ns |  1.22 |    0.04 |
|         |               |           |              |             |             |              |       |         |
|     Seq |            10 |         1 |    329.78 ns |  10.9671 ns |  31.6427 ns |    314.01 ns |  1.00 |    0.00 |
| Cistern |            10 |         1 |    209.45 ns |   2.7540 ns |   2.5761 ns |    210.42 ns |  0.55 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |            10 |         5 |    173.61 ns |   1.2068 ns |   1.1288 ns |    173.78 ns |  1.00 |    0.00 |
| Cistern |            10 |         5 |    146.71 ns |   2.1012 ns |   1.9654 ns |    147.72 ns |  0.85 |    0.01 |
|         |               |           |              |             |             |              |       |         |
|     Seq |            10 |        10 |    153.07 ns |   2.1891 ns |   2.0477 ns |    154.08 ns |  1.00 |    0.00 |
| Cistern |            10 |        10 |    131.64 ns |   1.7493 ns |   1.6363 ns |    132.17 ns |  0.86 |    0.02 |
|         |               |           |              |             |             |              |       |         |
|     Seq |          1000 |         1 | 23,299.36 ns | 217.9663 ns | 203.8858 ns | 23,297.38 ns |  1.00 |    0.00 |
| Cistern |          1000 |         1 | 11,047.74 ns | 124.0056 ns | 115.9949 ns | 11,021.93 ns |  0.47 |    0.01 |
|         |               |           |              |             |             |              |       |         |
|     Seq |          1000 |         5 | 10,553.08 ns | 110.2442 ns | 103.1225 ns | 10,533.13 ns |  1.00 |    0.00 |
| Cistern |          1000 |         5 |  5,217.57 ns |  51.9383 ns |  48.5831 ns |  5,212.98 ns |  0.49 |    0.01 |
|         |               |           |              |             |             |              |       |         |
|     Seq |          1000 |        10 |  8,914.24 ns | 126.3453 ns | 118.1835 ns |  8,963.85 ns |  1.00 |    0.00 |
| Cistern |          1000 |        10 |  4,387.41 ns |  57.6944 ns |  53.9674 ns |  4,416.96 ns |  0.49 |    0.01 |
*)
type ChunkBySize_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.length

(*
|  Method | NumberOfItems | ChunkSize |         Mean |       Error |      StdDev | Ratio |
|-------- |-------------- |---------- |-------------:|------------:|------------:|------:|
|     Seq |             0 |         1 |     50.80 ns |   0.1627 ns |   0.1522 ns |  1.00 |
| Cistern |             0 |         1 |     34.56 ns |   0.1061 ns |   0.0941 ns |  0.68 |
|         |               |           |              |             |             |       |
|     Seq |             0 |         5 |     50.80 ns |   0.1836 ns |   0.1717 ns |  1.00 |
| Cistern |             0 |         5 |     38.53 ns |   0.1715 ns |   0.1604 ns |  0.76 |
|         |               |           |              |             |             |       |
|     Seq |             0 |        10 |     50.94 ns |   0.0742 ns |   0.0694 ns |  1.00 |
| Cistern |             0 |        10 |     33.01 ns |   0.0850 ns |   0.0753 ns |  0.65 |
|         |               |           |              |             |             |       |
|     Seq |             1 |         1 |     82.19 ns |   0.3336 ns |   0.3121 ns |  1.00 |
| Cistern |             1 |         1 |     98.53 ns |   0.2641 ns |   0.2341 ns |  1.20 |
|         |               |           |              |             |             |       |
|     Seq |             1 |         5 |     93.38 ns |   0.5274 ns |   0.4675 ns |  1.00 |
| Cistern |             1 |         5 |    129.22 ns |   0.5955 ns |   0.4973 ns |  1.38 |
|         |               |           |              |             |             |       |
|     Seq |             1 |        10 |     96.49 ns |   0.2491 ns |   0.2209 ns |  1.00 |
| Cistern |             1 |        10 |    132.20 ns |   0.3691 ns |   0.3272 ns |  1.37 |
|         |               |           |              |             |             |       |
|     Seq |            10 |         1 |    355.88 ns |   1.8522 ns |   1.6419 ns |  1.00 |
| Cistern |            10 |         1 |    278.11 ns |   0.8647 ns |   0.8088 ns |  0.78 |
|         |               |           |              |             |             |       |
|     Seq |            10 |         5 |    173.10 ns |   0.6198 ns |   0.5798 ns |  1.00 |
| Cistern |            10 |         5 |    155.41 ns |   0.5510 ns |   0.4884 ns |  0.90 |
|         |               |           |              |             |             |       |
|     Seq |            10 |        10 |    149.56 ns |   0.4523 ns |   0.4231 ns |  1.00 |
| Cistern |            10 |        10 |    145.40 ns |   0.6371 ns |   0.5959 ns |  0.97 |
|         |               |           |              |             |             |       |
|     Seq |          1000 |         1 | 28,170.70 ns | 112.6347 ns | 105.3586 ns |  1.00 |
| Cistern |          1000 |         1 | 18,318.00 ns |  92.6474 ns |  82.1295 ns |  0.65 |
|         |               |           |              |             |             |       |
|     Seq |          1000 |         5 | 11,831.60 ns |  42.0078 ns |  39.2941 ns |  1.00 |
| Cistern |          1000 |         5 |  7,676.21 ns |  22.1903 ns |  19.6711 ns |  0.65 |
|         |               |           |              |             |             |       |
|     Seq |          1000 |        10 |  9,688.23 ns |  49.0580 ns |  45.8889 ns |  1.00 |
| Cistern |          1000 |        10 |  6,300.47 ns |  12.4328 ns |  11.6296 ns |  0.65 |
*)
type ChunkBySize_LengthByGetEnumerable() =
    inherit ChunkBySizeBase ()

    let length (chunks:seq<array<float>>) =
        let mutable count = 0
        for _ in chunks do
            count <- count + 1
        count

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> length

(*
|  Method | NumberOfItems | ChunkSize |        Mean |       Error |      StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |---------- |------------:|------------:|------------:|------:|--------:|------:|------:|----------:|
|     Seq |             0 |         1 |    124.8 ns |   0.4223 ns |   0.3744 ns |  1.00 |  0.0889 |     - |     - |     280 B |
| Cistern |             0 |         1 |    117.1 ns |   0.3264 ns |   0.2894 ns |  0.94 |  0.0432 |     - |     - |     136 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |             0 |         5 |    124.8 ns |   0.8575 ns |   0.7602 ns |  1.00 |  0.0889 |     - |     - |     280 B |
| Cistern |             0 |         5 |    113.7 ns |   0.3923 ns |   0.3669 ns |  0.91 |  0.0432 |     - |     - |     136 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |             0 |        10 |    124.0 ns |   0.4331 ns |   0.3840 ns |  1.00 |  0.0889 |     - |     - |     280 B |
| Cistern |             0 |        10 |    113.4 ns |   0.3403 ns |   0.3183 ns |  0.91 |  0.0432 |     - |     - |     136 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |             1 |         1 |    170.4 ns |   0.5169 ns |   0.4835 ns |  1.00 |  0.1168 |     - |     - |     368 B |
| Cistern |             1 |         1 |    249.8 ns |   0.9513 ns |   0.8433 ns |  1.47 |  0.1192 |     - |     - |     376 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |             1 |         5 |    182.4 ns |   0.7527 ns |   0.6286 ns |  1.00 |  0.1371 |     - |     - |     432 B |
| Cistern |             1 |         5 |    269.3 ns |   1.1974 ns |   0.9999 ns |  1.48 |  0.1397 |     - |     - |     440 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |             1 |        10 |    185.5 ns |   0.3972 ns |   0.3522 ns |  1.00 |  0.1500 |     - |     - |     472 B |
| Cistern |             1 |        10 |    281.1 ns |   1.0471 ns |   0.8175 ns |  1.52 |  0.1521 |     - |     - |     480 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |            10 |         1 |    541.6 ns |   2.2880 ns |   2.1402 ns |  1.00 |  0.2766 |     - |     - |     872 B |
| Cistern |            10 |         1 |    389.1 ns |   1.0630 ns |   0.8877 ns |  0.72 |  0.2108 |     - |     - |     664 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |            10 |         5 |    276.1 ns |   0.7617 ns |   0.7125 ns |  1.00 |  0.1550 |     - |     - |     488 B |
| Cistern |            10 |         5 |    304.2 ns |   1.0560 ns |   0.9878 ns |  1.10 |  0.1497 |     - |     - |     472 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |            10 |        10 |    241.1 ns |   0.9399 ns |   0.8792 ns |  1.00 |  0.1397 |     - |     - |     440 B |
| Cistern |            10 |        10 |    281.4 ns |   1.0559 ns |   0.9877 ns |  1.17 |  0.1421 |     - |     - |     448 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |          1000 |         1 | 38,436.3 ns | 134.6493 ns | 119.3631 ns |  1.00 | 17.8833 |     - |     - |   56312 B |
| Cistern |          1000 |         1 | 14,007.9 ns |  37.8086 ns |  35.3662 ns |  0.36 | 10.2692 |     - |     - |   32344 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |          1000 |         5 | 13,631.1 ns |  47.9909 ns |  40.0746 ns |  1.00 |  5.6915 |     - |     - |   17912 B |
| Cistern |          1000 |         5 |  6,510.1 ns |  30.5356 ns |  27.0691 ns |  0.48 |  4.1733 |     - |     - |   13144 B |
|         |               |           |             |             |             |       |         |       |       |           |
|     Seq |          1000 |        10 | 10,568.5 ns |  31.2916 ns |  29.2702 ns |  1.00 |  4.1656 |     - |     - |   13112 B |
| Cistern |          1000 |        10 |  5,416.3 ns |  24.1384 ns |  22.5791 ns |  0.51 |  3.4103 |     - |     - |   10744 B |
*)
[<CoreJob; MemoryDiagnoser>]
type ChunkBySize_Map_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.map id |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.map id |> Linq.length


(*
|  Method | NumberOfItems | ChunkSize |         Mean |         Error |        StdDev |       Median | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |---------- |-------------:|--------------:|--------------:|-------------:|------:|--------:|--------:|------:|------:|----------:|
|     Seq |             0 |         1 |    115.50 ns |     1.3410 ns |     1.2543 ns |    115.95 ns |  1.00 |    0.00 |  0.0941 |     - |     - |     296 B |
| Cistern |             0 |         1 |     92.89 ns |     0.8034 ns |     0.7515 ns |     93.12 ns |  0.80 |    0.01 |  0.0432 |     - |     - |     136 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |             0 |         5 |    114.38 ns |     1.3205 ns |     1.2352 ns |    113.95 ns |  1.00 |    0.00 |  0.0941 |     - |     - |     296 B |
| Cistern |             0 |         5 |     96.34 ns |     1.1881 ns |     1.1113 ns |     96.87 ns |  0.84 |    0.01 |  0.0432 |     - |     - |     136 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |             0 |        10 |    113.84 ns |     1.6733 ns |     1.5652 ns |    113.06 ns |  1.00 |    0.00 |  0.0941 |     - |     - |     296 B |
| Cistern |             0 |        10 |     95.77 ns |     1.1586 ns |     1.0837 ns |     96.08 ns |  0.84 |    0.02 |  0.0432 |     - |     - |     136 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |             1 |         1 |    154.83 ns |     1.7601 ns |     1.5603 ns |    154.98 ns |  1.00 |    0.00 |  0.1218 |     - |     - |     384 B |
| Cistern |             1 |         1 |    198.07 ns |     2.0565 ns |     1.9236 ns |    198.44 ns |  1.28 |    0.02 |  0.1194 |     - |     - |     376 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |             1 |         5 |    169.17 ns |     0.6040 ns |     0.5650 ns |    169.14 ns |  1.00 |    0.00 |  0.1423 |     - |     - |     448 B |
| Cistern |             1 |         5 |    227.98 ns |     3.9548 ns |     3.5058 ns |    227.52 ns |  1.35 |    0.02 |  0.1397 |     - |     - |     440 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |             1 |        10 |    170.52 ns |     1.2813 ns |     1.1985 ns |    170.87 ns |  1.00 |    0.00 |  0.1550 |     - |     - |     488 B |
| Cistern |             1 |        10 |    233.25 ns |     3.1999 ns |     2.9932 ns |    234.39 ns |  1.37 |    0.02 |  0.1523 |     - |     - |     480 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |            10 |         1 |    475.32 ns |     7.0548 ns |     6.5990 ns |    478.53 ns |  1.00 |    0.00 |  0.2813 |     - |     - |     888 B |
| Cistern |            10 |         1 |    330.65 ns |     4.1924 ns |     3.9216 ns |    329.97 ns |  0.70 |    0.01 |  0.2108 |     - |     - |     664 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |            10 |         5 |    254.86 ns |     3.7468 ns |     3.5048 ns |    255.32 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
| Cistern |            10 |         5 |    249.81 ns |     3.4533 ns |     3.2302 ns |    251.02 ns |  0.98 |    0.02 |  0.1497 |     - |     - |     472 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |            10 |        10 |    226.20 ns |     2.1912 ns |     2.0497 ns |    227.13 ns |  1.00 |    0.00 |  0.1450 |     - |     - |     456 B |
| Cistern |            10 |        10 |    239.41 ns |     2.9334 ns |     2.6004 ns |    240.71 ns |  1.06 |    0.01 |  0.1421 |     - |     - |     448 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |          1000 |         1 | 37,030.47 ns | 1,219.0926 ns | 3,497.8052 ns | 35,409.26 ns |  1.00 |    0.00 | 17.8833 |     - |     - |   56328 B |
| Cistern |          1000 |         1 | 13,480.24 ns |   157.9527 ns |   140.0209 ns | 13,452.17 ns |  0.34 |    0.03 | 10.2692 |     - |     - |   32344 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |          1000 |         5 | 12,629.89 ns |   167.8592 ns |   157.0156 ns | 12,641.70 ns |  1.00 |    0.00 |  5.6915 |     - |     - |   17928 B |
| Cistern |          1000 |         5 |  5,874.26 ns |    73.4071 ns |    61.2983 ns |  5,873.84 ns |  0.46 |    0.01 |  4.1733 |     - |     - |   13144 B |
|         |               |           |              |               |               |              |       |         |         |       |       |           |
|     Seq |          1000 |        10 |  9,955.48 ns |   114.2148 ns |   106.8366 ns | 10,002.26 ns |  1.00 |    0.00 |  4.1656 |     - |     - |   13128 B |
| Cistern |          1000 |        10 |  4,859.18 ns |    27.3554 ns |    24.2499 ns |  4,864.19 ns |  0.49 |    0.00 |  3.4103 |     - |     - |   10744 B |
*)
[<CoreJob; MemoryDiagnoser>]
type ChunkBySize_Filter_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.filter (fun _ -> true) |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.filter (fun _ -> true) |> Linq.length
