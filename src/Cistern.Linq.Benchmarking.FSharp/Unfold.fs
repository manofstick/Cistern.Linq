namespace Cistern.Linq.Benchmarking.FSharp.Unfold

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

type UnfoldFSharpListBase () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

(*
|  Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|
|     Seq |             0 |           NA |         NA |         NA |     ? |       ? |
| Cistern |             0 |           NA |         NA |         NA |     ? |       ? |
|         |               |              |            |            |       |         |
|     Seq |             1 |     72.00 ns |  0.2616 ns |  0.2319 ns |  1.00 |    0.00 |
| Cistern |             1 |     58.39 ns |  0.1850 ns |  0.1640 ns |  0.81 |    0.00 |
|         |               |              |            |            |       |         |
|     Seq |            10 |    276.85 ns |  0.8193 ns |  0.7664 ns |  1.00 |    0.00 |
| Cistern |            10 |    205.80 ns |  0.4724 ns |  0.4188 ns |  0.74 |    0.00 |
|         |               |              |            |            |       |         |
|     Seq |          1000 | 21,571.04 ns | 54.1420 ns | 47.9954 ns |  1.00 |    0.00 |
| Cistern |          1000 | 14,768.15 ns | 34.4868 ns | 30.5716 ns |  0.68 |    0.00 |
*)
type UnfoldFSharpList_Max() =
    inherit UnfoldFSharpListBase ()

    member private __.folder x =
        if x < base.NumberOfItems
        then Some (float x, x+1)
        else None

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.unfold this.folder 0 |> Seq.max

    [<Benchmark>]
    member this.Cistern () =  Linq.unfold this.folder 0 |> Linq.max
