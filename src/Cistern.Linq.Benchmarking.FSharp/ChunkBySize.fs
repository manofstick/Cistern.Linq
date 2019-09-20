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
|  Method | NumberOfItems | ChunkSize |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |---------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |         1 |     64.34 ns |   0.3809 ns |   0.3563 ns |  1.00 |    0.00 |
| Cistern |             0 |         1 |    147.12 ns |   1.2072 ns |   1.1292 ns |  2.29 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |             0 |         5 |     64.47 ns |   0.4579 ns |   0.4283 ns |  1.00 |    0.00 |
| Cistern |             0 |         5 |    150.70 ns |   0.7460 ns |   0.6978 ns |  2.34 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |             0 |        10 |     64.24 ns |   0.6811 ns |   0.6371 ns |  1.00 |    0.00 |
| Cistern |             0 |        10 |    150.43 ns |   0.8426 ns |   0.7881 ns |  2.34 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |         1 |     97.14 ns |   0.5447 ns |   0.5095 ns |  1.00 |    0.00 |
| Cistern |             1 |         1 |    168.43 ns |   1.1807 ns |   1.1045 ns |  1.73 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |         5 |    109.27 ns |   0.6184 ns |   0.5785 ns |  1.00 |    0.00 |
| Cistern |             1 |         5 |    182.63 ns |   1.2894 ns |   1.2061 ns |  1.67 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |        10 |    113.32 ns |   0.4962 ns |   0.4641 ns |  1.00 |    0.00 |
| Cistern |             1 |        10 |    187.66 ns |   2.3965 ns |   2.1244 ns |  1.66 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |         1 |    350.29 ns |   2.1799 ns |   2.0391 ns |  1.00 |    0.00 |
| Cistern |            10 |         1 |    381.03 ns |   2.5991 ns |   2.4312 ns |  1.09 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |         5 |    191.67 ns |   1.1805 ns |   1.1042 ns |  1.00 |    0.00 |
| Cistern |            10 |         5 |    250.27 ns |   1.3265 ns |   1.2408 ns |  1.31 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |        10 |    169.77 ns |   1.5776 ns |   1.4757 ns |  1.00 |    0.00 |
| Cistern |            10 |        10 |    242.03 ns |   1.4275 ns |   1.3353 ns |  1.43 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |         1 | 26,132.23 ns | 159.8823 ns | 149.5540 ns |  1.00 |    0.00 |
| Cistern |          1000 |         1 | 18,333.26 ns | 126.1618 ns | 118.0118 ns |  0.70 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |         5 | 11,838.64 ns |  79.2920 ns |  74.1698 ns |  1.00 |    0.00 |
| Cistern |          1000 |         5 |  9,461.41 ns |  56.7079 ns |  53.0446 ns |  0.80 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |        10 |  9,923.39 ns |  60.8141 ns |  56.8855 ns |  1.00 |    0.00 |
| Cistern |          1000 |        10 |  9,666.22 ns |  66.5265 ns |  62.2289 ns |  0.97 |    0.01 |
*)
type ChunkBySize_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.length

(*
|  Method | NumberOfItems | ChunkSize |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |---------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |         1 |     53.55 ns |   0.2854 ns |   0.2670 ns |  1.00 |    0.00 |
| Cistern |             0 |         1 |    116.16 ns |   0.8203 ns |   0.7673 ns |  2.17 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |             0 |         5 |     53.80 ns |   0.1868 ns |   0.1656 ns |  1.00 |    0.00 |
| Cistern |             0 |         5 |    117.58 ns |   0.8454 ns |   0.7908 ns |  2.19 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |             0 |        10 |     53.79 ns |   0.3755 ns |   0.3328 ns |  1.00 |    0.00 |
| Cistern |             0 |        10 |    122.23 ns |   0.8647 ns |   0.8089 ns |  2.27 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |         1 |     87.00 ns |   0.3841 ns |   0.3405 ns |  1.00 |    0.00 |
| Cistern |             1 |         1 |    158.52 ns |   0.7967 ns |   0.7452 ns |  1.82 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |         5 |     98.87 ns |   0.5375 ns |   0.4765 ns |  1.00 |    0.00 |
| Cistern |             1 |         5 |    176.15 ns |   1.0645 ns |   0.9957 ns |  1.78 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |             1 |        10 |    102.75 ns |   0.7585 ns |   0.7095 ns |  1.00 |    0.00 |
| Cistern |             1 |        10 |    179.28 ns |   1.1383 ns |   1.0648 ns |  1.74 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |         1 |    374.18 ns |   2.4398 ns |   2.2822 ns |  1.00 |    0.00 |
| Cistern |            10 |         1 |    511.84 ns |   3.9745 ns |   3.7177 ns |  1.37 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |         5 |    182.97 ns |   1.3769 ns |   1.2879 ns |  1.00 |    0.00 |
| Cistern |            10 |         5 |    261.54 ns |   1.3503 ns |   1.1970 ns |  1.43 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |            10 |        10 |    156.98 ns |   1.0533 ns |   0.9853 ns |  1.00 |    0.00 |
| Cistern |            10 |        10 |    236.08 ns |   1.9611 ns |   1.8344 ns |  1.50 |    0.02 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |         1 | 29,632.48 ns | 125.1735 ns | 117.0873 ns |  1.00 |    0.00 |
| Cistern |          1000 |         1 | 36,778.09 ns | 247.3449 ns | 231.3666 ns |  1.24 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |         5 | 12,660.67 ns |  79.0876 ns |  70.1091 ns |  1.00 |    0.00 |
| Cistern |          1000 |         5 | 13,493.21 ns | 104.2990 ns |  97.5614 ns |  1.07 |    0.01 |
|         |               |           |              |             |             |       |         |
|     Seq |          1000 |        10 | 10,239.91 ns |  61.7410 ns |  57.7525 ns |  1.00 |    0.00 |
| Cistern |          1000 |        10 | 10,748.24 ns |  66.6733 ns |  62.3662 ns |  1.05 |    0.01 |
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
*)
[<CoreJob; MemoryDiagnoser>]
type ChunkBySize_Map_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.map id |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.map id |> Linq.length


(*
|  Method | NumberOfItems | ChunkSize |        Mean |       Error |      StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |---------- |------------:|------------:|------------:|------:|--------:|--------:|------:|------:|----------:|
|     Seq |             0 |         1 |    129.3 ns |   1.0571 ns |   0.9888 ns |  1.00 |    0.00 |  0.0939 |     - |     - |     296 B |
| Cistern |             0 |         1 |    222.6 ns |   1.8859 ns |   1.7640 ns |  1.72 |    0.02 |  0.0837 |     - |     - |     264 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |             0 |         5 |    129.2 ns |   1.2119 ns |   1.1336 ns |  1.00 |    0.00 |  0.0939 |     - |     - |     296 B |
| Cistern |             0 |         5 |    223.5 ns |   1.8419 ns |   1.7229 ns |  1.73 |    0.02 |  0.0939 |     - |     - |     296 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |             0 |        10 |    129.2 ns |   0.9224 ns |   0.8629 ns |  1.00 |    0.00 |  0.0939 |     - |     - |     296 B |
| Cistern |             0 |        10 |    223.8 ns |   1.4990 ns |   1.3289 ns |  1.73 |    0.02 |  0.1066 |     - |     - |     336 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |             1 |         1 |    174.8 ns |   1.2211 ns |   1.1422 ns |  1.00 |    0.00 |  0.1218 |     - |     - |     384 B |
| Cistern |             1 |         1 |    242.1 ns |   1.8770 ns |   1.6639 ns |  1.39 |    0.01 |  0.1040 |     - |     - |     328 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |             1 |         5 |    188.2 ns |   1.0913 ns |   1.0208 ns |  1.00 |    0.00 |  0.1423 |     - |     - |     448 B |
| Cistern |             1 |         5 |    260.1 ns |   2.1029 ns |   1.7560 ns |  1.38 |    0.01 |  0.1140 |     - |     - |     360 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |             1 |        10 |    190.8 ns |   1.5289 ns |   1.4301 ns |  1.00 |    0.00 |  0.1550 |     - |     - |     488 B |
| Cistern |             1 |        10 |    265.5 ns |   2.1378 ns |   1.7851 ns |  1.39 |    0.01 |  0.1268 |     - |     - |     400 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |            10 |         1 |    535.0 ns |   3.2881 ns |   3.0756 ns |  1.00 |    0.00 |  0.2813 |     - |     - |     888 B |
| Cistern |            10 |         1 |    423.1 ns |   2.6058 ns |   2.3100 ns |  0.79 |    0.01 |  0.1955 |     - |     - |     616 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |            10 |         5 |    288.2 ns |   1.2524 ns |   1.1102 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
| Cistern |            10 |         5 |    316.9 ns |   2.5686 ns |   2.4027 ns |  1.10 |    0.01 |  0.1445 |     - |     - |     456 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |            10 |        10 |    258.1 ns |   1.7347 ns |   1.6226 ns |  1.00 |    0.00 |  0.1450 |     - |     - |     456 B |
| Cistern |            10 |        10 |    308.6 ns |   1.6003 ns |   1.4969 ns |  1.20 |    0.01 |  0.1497 |     - |     - |     472 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |          1000 |         1 | 37,583.4 ns | 302.3257 ns | 282.7957 ns |  1.00 |    0.00 | 17.8833 |     - |     - |   56328 B |
| Cistern |          1000 |         1 | 18,564.1 ns | 117.5420 ns | 109.9489 ns |  0.49 |    0.00 | 10.2539 |     - |     - |   32296 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |          1000 |         5 | 14,232.1 ns |  88.9887 ns |  83.2401 ns |  1.00 |    0.00 |  5.6915 |     - |     - |   17928 B |
| Cistern |          1000 |         5 |  8,972.7 ns |  53.7137 ns |  47.6158 ns |  0.63 |    0.01 |  4.1656 |     - |     - |   13128 B |
|         |               |           |             |             |             |       |         |         |       |       |           |
|     Seq |          1000 |        10 | 11,083.2 ns |  63.9200 ns |  59.7908 ns |  1.00 |    0.00 |  4.1656 |     - |     - |   13128 B |
| Cistern |          1000 |        10 |  9,015.0 ns |  47.6941 ns |  44.6131 ns |  0.81 |    0.01 |  3.4180 |     - |     - |   10768 B |
*)
[<CoreJob; MemoryDiagnoser>]
type ChunkBySize_Filter_Length() =
    inherit ChunkBySizeBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.chunkBySize this.ChunkSize this.Source |> Seq.filter (fun _ -> true) |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.chunkBySize this.ChunkSize this.Source |> Linq.filter (fun _ -> true) |> Linq.length
