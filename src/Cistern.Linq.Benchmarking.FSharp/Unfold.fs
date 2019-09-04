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

    member this.folderV x =
        if x < this.NumberOfItems
        then ValueSome (float x, x+1)
        else ValueNone


(*
|   Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|--------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|      Seq |             0 |           NA |          NA |          NA |     ? |       ? |
|  Cistern |             0 |           NA |          NA |          NA |     ? |       ? |
| CisternV |             0 |           NA |          NA |          NA |     ? |       ? |
|          |               |              |             |             |       |         |
|      Seq |             1 |     77.00 ns |   0.5107 ns |   0.4777 ns |  1.00 |    0.00 |
|  Cistern |             1 |     88.66 ns |   0.6359 ns |   0.5637 ns |  1.15 |    0.01 |
| CisternV |             1 |     83.91 ns |   0.6043 ns |   0.5357 ns |  1.09 |    0.01 |
|          |               |              |             |             |       |         |
|      Seq |            10 |    296.98 ns |   1.6036 ns |   1.5000 ns |  1.00 |    0.00 |
|  Cistern |            10 |    254.52 ns |   1.4742 ns |   1.3789 ns |  0.86 |    0.01 |
| CisternV |            10 |    202.45 ns |   0.7582 ns |   0.6721 ns |  0.68 |    0.00 |
|          |               |              |             |             |       |         |
|      Seq |          1000 | 23,114.49 ns |  91.4209 ns |  76.3406 ns |  1.00 |    0.00 |
|  Cistern |          1000 | 16,165.86 ns | 107.6515 ns | 100.6973 ns |  0.70 |    0.00 |
| CisternV |          1000 | 11,340.76 ns |  47.8941 ns |  44.8002 ns |  0.49 |    0.00 |
*)
type UnfoldFSharpList_Max() =
    inherit UnfoldFSharpListBase ()


    [<Benchmark (Baseline = true)>]
    member this.Seq () = Seq.unfold this.folder 0 |> Seq.max

    [<Benchmark>]
    member this.Cistern () = Linq.unfold this.folder 0 |> Linq.max

    [<Benchmark>]
    member this.CisternV () = Linq.unfoldV this.folderV 0 |> Linq.max

(*
|   Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio |
|--------- |-------------- |-------------:|-----------:|-----------:|------:|
|      Seq |             0 |     52.86 ns |  0.1897 ns |  0.1584 ns |  1.00 |
|  Cistern |             0 |     72.74 ns |  0.4639 ns |  0.4113 ns |  1.38 |
| CisternV |             0 |     71.11 ns |  0.6329 ns |  0.5920 ns |  1.35 |
|          |               |              |            |            |       |
|      Seq |             1 |     78.19 ns |  0.3147 ns |  0.2789 ns |  1.00 |
|  Cistern |             1 |     93.74 ns |  0.3793 ns |  0.3548 ns |  1.20 |
| CisternV |             1 |     89.72 ns |  0.4435 ns |  0.4148 ns |  1.15 |
|          |               |              |            |            |       |
|      Seq |            10 |    311.75 ns |  1.8503 ns |  1.7308 ns |  1.00 |
|  Cistern |            10 |    297.48 ns |  1.0020 ns |  0.9373 ns |  0.95 |
| CisternV |            10 |    260.58 ns |  1.5903 ns |  1.4876 ns |  0.84 |
|          |               |              |            |            |       |
|      Seq |          1000 | 24,400.05 ns | 76.4057 ns | 63.8022 ns |  1.00 |
|  Cistern |          1000 | 21,514.93 ns | 75.8685 ns | 70.9675 ns |  0.88 |
| CisternV |          1000 | 17,973.90 ns | 67.1035 ns | 62.7687 ns |  0.74 |
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

    [<Benchmark>]
    member this.CisternV () = Linq.unfoldV this.folderV 0 |> max

