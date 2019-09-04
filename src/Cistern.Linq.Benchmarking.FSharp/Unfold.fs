namespace Cistern.Linq.Benchmarking.FSharp.Unfold

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

type UnfoldFSharpListBase () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member this.folder x =
        if x < this.NumberOfItems
        then Some (float x, x+1)
        else None


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


    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.unfold this.folder 0 |> Seq.max

    [<Benchmark>]
    member this.Cistern () =  Linq.unfold this.folder 0 |> Linq.max

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |     56.36 ns |   0.5472 ns |   0.4851 ns |  1.00 |    0.00 |
| Cistern |             0 |     89.20 ns |   0.4938 ns |   0.4377 ns |  1.58 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     77.39 ns |   0.5153 ns |   0.4820 ns |  1.00 |    0.00 |
| Cistern |             1 |    115.28 ns |   1.0779 ns |   1.0083 ns |  1.49 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    296.87 ns |   2.0273 ns |   1.8963 ns |  1.00 |    0.00 |
| Cistern |            10 |    364.74 ns |   1.6430 ns |   1.5369 ns |  1.23 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 23,108.10 ns |  79.7722 ns |  74.6190 ns |  1.00 |    0.00 |
| Cistern |          1000 | 26,190.79 ns | 117.5903 ns | 109.9941 ns |  1.13 |    0.01 |
*)
type UnfoldFSharpList_MaxByGetEnumerable() =
    inherit UnfoldFSharpListBase ()

    let max (unfold:seq<float>) =
        let mutable max = System.Double.MinValue
        for x in unfold do
            if x > max then
                max <- x
        max

    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.unfold this.folder 0 |> max

    [<Benchmark>]
    member this.Cistern () = Linq.unfold this.folder 0 |> max

