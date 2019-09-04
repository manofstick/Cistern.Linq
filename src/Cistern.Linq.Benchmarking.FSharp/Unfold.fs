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
|   Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio |
|--------- |-------------- |-------------:|------------:|------------:|------:|
|      Seq |             0 |     53.28 ns |   0.2576 ns |   0.2410 ns |  1.00 |
|  Cistern |             0 |     70.58 ns |   0.4990 ns |   0.4424 ns |  1.33 |
| CisternV |             0 |     66.06 ns |   0.3932 ns |   0.3678 ns |  1.24 |
|          |               |              |             |             |       |
|      Seq |             1 |     77.79 ns |   0.4980 ns |   0.4658 ns |  1.00 |
|  Cistern |             1 |     91.80 ns |   0.4062 ns |   0.3799 ns |  1.18 |
| CisternV |             1 |     87.75 ns |   0.3039 ns |   0.2694 ns |  1.13 |
|          |               |              |             |             |       |
|      Seq |            10 |    311.51 ns |   1.3073 ns |   1.2229 ns |  1.00 |
|  Cistern |            10 |    311.80 ns |   1.2284 ns |   1.1490 ns |  1.00 |
| CisternV |            10 |    283.81 ns |   1.4155 ns |   1.3241 ns |  0.91 |
|          |               |              |             |             |       |
|      Seq |          1000 | 24,493.90 ns | 107.6336 ns | 100.6805 ns |  1.00 |
|  Cistern |          1000 | 23,136.30 ns | 105.8261 ns |  98.9898 ns |  0.94 |
| CisternV |          1000 | 19,713.27 ns | 107.2004 ns | 100.2753 ns |  0.80 |
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

