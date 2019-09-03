namespace Cistern.Linq.Benchmarking.FSharp.Vanilla

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

type VanillaFSharpListBase () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Numbers = [] with get, set

    [<GlobalSetup>]
    member this.Setup() =
        Cistern.Linq.FSharp.Register.RegisterFSharpCollections ()

        this.Numbers <-
            List.init this.NumberOfItems (fun x -> float (x + 1))

(*
|  Method | NumberOfItems |          Mean |      Error |     StdDev | Ratio | RatioSD |
|-------- |-------------- |--------------:|-----------:|-----------:|------:|--------:|
|    List |             0 |            NA |         NA |         NA |     ? |       ? |
|     Seq |             0 |            NA |         NA |         NA |     ? |       ? |
| Cistern |             0 |            NA |         NA |         NA |     ? |       ? |
|         |               |               |            |            |       |         |
|    List |             1 |     0.5792 ns |  0.0137 ns |  0.0128 ns |  0.03 |    0.00 |
|     Seq |             1 |    21.2482 ns |  0.1214 ns |  0.1135 ns |  1.00 |    0.00 |
| Cistern |             1 |    96.7574 ns |  0.5602 ns |  0.5240 ns |  4.55 |    0.04 |
|         |               |               |            |            |       |         |
|    List |            10 |     7.2266 ns |  0.0567 ns |  0.0531 ns |  0.07 |    0.00 |
|     Seq |            10 |   103.9565 ns |  0.5477 ns |  0.5124 ns |  1.00 |    0.00 |
| Cistern |            10 |   111.2057 ns |  0.6349 ns |  0.5628 ns |  1.07 |    0.01 |
|         |               |               |            |            |       |         |
|    List |          1000 | 1,388.9021 ns |  9.7860 ns |  8.6750 ns |  0.17 |    0.00 |
|     Seq |          1000 | 8,115.9833 ns | 59.8701 ns | 56.0026 ns |  1.00 |    0.00 |
| Cistern |          1000 | 1,840.7250 ns | 38.9822 ns | 61.8299 ns |  0.23 |    0.01 |
*)
type VanillaFSharpList_Max() =
    inherit VanillaFSharpListBase ()

    [<Benchmark>]
    member this.List () = this.Numbers |> List.max

    [<Benchmark (Baseline = true)>]
    member this.Seq () = this.Numbers |> Seq.max

    [<Benchmark>]
    member this.Cistern () = this.Numbers |> Linq.max
