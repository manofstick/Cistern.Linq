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
|     Seq |             0 |     57.46 ns |  0.2094 ns |  0.1959 ns |  1.00 |
| Cistern |             0 |     49.62 ns |  0.1610 ns |  0.1506 ns |  0.86 |
|         |               |              |            |            |       |
|     Seq |             1 |     70.74 ns |  0.2131 ns |  0.1664 ns |  1.00 |
| Cistern |             1 |     89.15 ns |  0.2449 ns |  0.2291 ns |  1.26 |
|         |               |              |            |            |       |
|     Seq |            10 |    246.28 ns |  0.5855 ns |  0.5477 ns |  1.00 |
| Cistern |            10 |    162.15 ns |  0.3494 ns |  0.3268 ns |  0.66 |
|         |               |              |            |            |       |
|     Seq |          1000 | 17,746.33 ns | 60.8149 ns | 56.8863 ns |  1.00 |
| Cistern |          1000 |  7,398.68 ns | 14.0180 ns | 12.4266 ns |  0.42 |
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
|     Seq |             0 |     51.19 ns |   0.7279 ns |   0.6809 ns |  1.00 |    0.00 |
| Cistern |             0 |     33.24 ns |   0.4211 ns |   0.3939 ns |  0.65 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     62.17 ns |   0.9244 ns |   0.8647 ns |  1.00 |    0.00 |
| Cistern |             1 |     80.87 ns |   0.4912 ns |   0.4595 ns |  1.30 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    270.91 ns |   3.6002 ns |   3.3677 ns |  1.00 |    0.00 |
| Cistern |            10 |    229.48 ns |   3.9240 ns |   3.6705 ns |  0.85 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 21,857.19 ns | 180.4387 ns | 168.7824 ns |  1.00 |    0.00 |
| Cistern |          1000 | 15,696.27 ns | 195.7857 ns | 183.1381 ns |  0.72 |    0.01 |
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
|  Method | NumberOfItems |        Mean |      Error |     StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |------------:|-----------:|-----------:|------:|--------:|--------:|------:|------:|----------:|
|     Seq |             0 |    119.1 ns |   2.015 ns |   1.885 ns |  1.00 |    0.00 |  0.0989 |     - |     - |     312 B |
| Cistern |             0 |    107.4 ns |   1.582 ns |   1.480 ns |  0.90 |    0.02 |  0.0355 |     - |     - |     112 B |
|         |               |             |            |            |       |         |         |       |       |           |
|     Seq |             1 |    133.4 ns |   1.378 ns |   1.289 ns |  1.00 |    0.00 |  0.1168 |     - |     - |     368 B |
| Cistern |             1 |    210.9 ns |   2.364 ns |   2.211 ns |  1.58 |    0.02 |  0.1092 |     - |     - |     344 B |
|         |               |             |            |            |       |         |         |       |       |           |
|     Seq |            10 |    430.2 ns |   6.179 ns |   5.780 ns |  1.00 |    0.00 |  0.2084 |     - |     - |     656 B |
| Cistern |            10 |    313.7 ns |   3.396 ns |   3.177 ns |  0.73 |    0.02 |  0.2007 |     - |     - |     632 B |
|         |               |             |            |            |       |         |         |       |       |           |
|     Seq |          1000 | 31,684.2 ns | 433.382 ns | 338.356 ns |  1.00 |    0.00 | 10.2539 |     - |     - |   32336 B |
| Cistern |          1000 |  9,957.4 ns | 114.844 ns | 107.425 ns |  0.31 |    0.00 | 10.2692 |     - |     - |   32312 B |*)
[<CoreJob; MemoryDiagnoser>]
type Pairwise_Map_Length() =
    inherit PairwiseBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> Seq.map id |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> Linq.map id |> Linq.length


(*
|  Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|--------:|------:|------:|----------:|
|     Seq |             0 |    121.65 ns |   1.682 ns |   1.574 ns |  1.00 |    0.00 |  0.1042 |     - |     - |     328 B |
| Cistern |             0 |     89.95 ns |   1.070 ns |   1.001 ns |  0.74 |    0.01 |  0.0355 |     - |     - |     112 B |
|         |               |              |            |            |       |         |         |       |       |           |
|     Seq |             1 |    131.63 ns |   1.836 ns |   1.717 ns |  1.00 |    0.00 |  0.1218 |     - |     - |     384 B |
| Cistern |             1 |    189.07 ns |   2.426 ns |   2.269 ns |  1.44 |    0.03 |  0.1092 |     - |     - |     344 B |
|         |               |              |            |            |       |         |         |       |       |           |
|     Seq |            10 |    414.85 ns |   3.695 ns |   3.456 ns |  1.00 |    0.00 |  0.2131 |     - |     - |     672 B |
| Cistern |            10 |    298.50 ns |   4.232 ns |   3.959 ns |  0.72 |    0.01 |  0.2007 |     - |     - |     632 B |
|         |               |              |            |            |       |         |         |       |       |           |
|     Seq |          1000 | 28,901.79 ns | 326.310 ns | 305.231 ns |  1.00 |    0.00 | 10.2539 |     - |     - |   32352 B |
| Cistern |          1000 | 10,568.62 ns | 148.852 ns | 139.236 ns |  0.37 |    0.01 | 10.2692 |     - |     - |   32312 B |
*)
[<CoreJob; MemoryDiagnoser>]
type Pairwise_Filter_Length() =
    inherit PairwiseBase ()

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.pairwise this.Source |> Seq.filter (fun _ -> true) |> Seq.length

    [<Benchmark>]
    member this.Cistern () = Linq.pairwise this.Source |> Linq.filter (fun _ -> true) |> Linq.length
