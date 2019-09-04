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
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |
|-------- |-------------- |-------------:|------------:|------------:|------:|
|     Seq |             0 |     54.09 ns |   0.2397 ns |   0.2125 ns |  1.00 |
| Cistern |             0 |     69.26 ns |   0.3024 ns |   0.2828 ns |  1.28 |
|         |               |              |             |             |       |
|     Seq |             1 |     78.83 ns |   0.4031 ns |   0.3771 ns |  1.00 |
| Cistern |             1 |     92.91 ns |   0.3931 ns |   0.3677 ns |  1.18 |
|         |               |              |             |             |       |
|     Seq |            10 |    308.99 ns |   1.3515 ns |   1.1980 ns |  1.00 |
| Cistern |            10 |    320.31 ns |   1.4706 ns |   1.3756 ns |  1.04 |
|         |               |              |             |             |       |
|     Seq |          1000 | 24,213.28 ns | 111.1952 ns | 104.0120 ns |  1.00 |
| Cistern |          1000 | 23,423.51 ns | 123.4586 ns | 115.4833 ns |  0.97 |
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

