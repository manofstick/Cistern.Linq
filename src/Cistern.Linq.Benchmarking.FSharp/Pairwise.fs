namespace Cistern.Linq.Benchmarking.FSharp.Pairwise

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

type PairwiseBase () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Source = [||] with get, set

    [<GlobalSetup>]
    member this.Setup() =
        this.Source <- Array.init this.NumberOfItems (fun x -> float (x + 1))

(*
|  Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio |
|-------- |-------------- |-------------:|-----------:|-----------:|------:|
|     Seq |             0 |     64.06 ns |  0.1741 ns |  0.1544 ns |  1.00 |
| Cistern |             0 |    110.10 ns |  0.4517 ns |  0.4004 ns |  1.72 |
|         |               |              |            |            |       |
|     Seq |             1 |     80.10 ns |  0.2651 ns |  0.2480 ns |  1.00 |
| Cistern |             1 |    113.97 ns |  0.3941 ns |  0.3494 ns |  1.42 |
|         |               |              |            |            |       |
|     Seq |            10 |    274.20 ns |  0.7736 ns |  0.7236 ns |  1.00 |
| Cistern |            10 |    266.52 ns |  0.6044 ns |  0.5654 ns |  0.97 |
|         |               |              |            |            |       |
|     Seq |          1000 | 19,720.66 ns | 53.5018 ns | 47.4279 ns |  1.00 |
| Cistern |          1000 | 14,831.40 ns | 41.6818 ns | 38.9892 ns |  0.75 |
*)
type Pairwise_Length() =
    inherit PairwiseBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> Linq.length

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |     54.11 ns |   0.3693 ns |   0.3454 ns |  1.00 |    0.00 |
| Cistern |             0 |    102.01 ns |   0.3857 ns |   0.3608 ns |  1.89 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     67.72 ns |   0.2672 ns |   0.2499 ns |  1.00 |    0.00 |
| Cistern |             1 |    108.50 ns |   0.3093 ns |   0.2742 ns |  1.60 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    293.84 ns |   1.7489 ns |   1.6359 ns |  1.00 |    0.00 |
| Cistern |            10 |    360.05 ns |   2.2512 ns |   1.8799 ns |  1.23 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 23,231.21 ns |  97.9512 ns |  91.6236 ns |  1.00 |    0.00 |
| Cistern |          1000 | 25,987.41 ns | 557.8105 ns | 465.7971 ns |  1.12 |    0.02 |
*)
type Pairwise_LengthByGetEnumerable() =
    inherit PairwiseBase ()

    let length (chunks:seq<_>) =
        let mutable count = 0
        for _ in chunks do
            count <- count + 1
        count

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> length

(*
|  Method | NumberOfItems |        Mean |       Error |      StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |------------:|------------:|------------:|------:|--------:|------:|------:|----------:|
|     Seq |             0 |    128.8 ns |   0.8722 ns |   0.8158 ns |  1.00 |  0.0989 |     - |     - |     312 B |
| Cistern |             0 |    183.0 ns |   0.4861 ns |   0.4547 ns |  1.42 |  0.0737 |     - |     - |     232 B |
|         |               |             |             |             |       |         |       |       |           |
|     Seq |             1 |    147.6 ns |   0.8090 ns |   0.7172 ns |  1.00 |  0.1168 |     - |     - |     368 B |
| Cistern |             1 |    188.5 ns |   1.5895 ns |   1.4091 ns |  1.28 |  0.0837 |     - |     - |     264 B |
|         |               |             |             |             |       |         |       |       |           |
|     Seq |            10 |    453.2 ns |   1.4671 ns |   1.3723 ns |  1.00 |  0.2084 |     - |     - |     656 B |
| Cistern |            10 |    354.0 ns |   1.6526 ns |   1.4650 ns |  0.78 |  0.1750 |     - |     - |     552 B |
|         |               |             |             |             |       |         |       |       |           |
|     Seq |          1000 | 32,782.1 ns | 119.8339 ns | 112.0927 ns |  1.00 | 10.2539 |     - |     - |   32336 B |
| Cistern |          1000 | 16,490.5 ns |  74.3968 ns |  69.5908 ns |  0.50 | 10.2234 |     - |     - |   32232 B |*)
[<CoreJob; MemoryDiagnoser>]
type Pairwise_Map_Length() =
    inherit PairwiseBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> Seq.map id |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> Linq.map id |> Linq.length


(*
|  Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |------------:|-----------:|-----------:|------:|--------:|------:|------:|----------:|
|     Seq |             0 |    125.8 ns |  0.5196 ns |  0.4860 ns |  1.00 |  0.1042 |     - |     - |     328 B |
| Cistern |             0 |    174.2 ns |  0.3564 ns |  0.3333 ns |  1.38 |  0.0737 |     - |     - |     232 B |
|         |               |             |            |            |       |         |       |       |           |
|     Seq |             1 |    138.6 ns |  0.3714 ns |  0.3474 ns |  1.00 |  0.1218 |     - |     - |     384 B |
| Cistern |             1 |    180.3 ns |  0.7545 ns |  0.7057 ns |  1.30 |  0.0837 |     - |     - |     264 B |
|         |               |             |            |            |       |         |       |       |           |
|     Seq |            10 |    432.5 ns |  1.2409 ns |  1.1607 ns |  1.00 |  0.2131 |     - |     - |     672 B |
| Cistern |            10 |    350.2 ns |  0.8826 ns |  0.7824 ns |  0.81 |  0.1750 |     - |     - |     552 B |
|         |               |             |            |            |       |         |       |       |           |
|     Seq |          1000 | 30,347.0 ns | 96.5064 ns | 90.2722 ns |  1.00 | 10.2539 |     - |     - |   32352 B |
| Cistern |          1000 | 17,046.6 ns | 65.7071 ns | 61.4624 ns |  0.56 | 10.2234 |     - |     - |   32232 B |
*)
[<CoreJob; MemoryDiagnoser>]
type Pairwise_Filter_Length() =
    inherit PairwiseBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> Seq.filter (fun _ -> true) |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> Linq.filter (fun _ -> true) |> Linq.length
