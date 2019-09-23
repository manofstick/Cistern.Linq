namespace Cistern.Linq.Benchmarking.FSharp.ValueOptionExtensions

// from:
// https://raw.githubusercontent.com/dotnet/fsharp/adc9a3c998460def17a5b1ef7373dd433755318f/tests/fsharp/perf/ValueOption/ValueOption/Program.fs

open BenchmarkDotNet.Attributes

open Cistern.Linq.FSharp

type Record = {
  Int: int
  String: string
  Children: Record list
}

[<AutoOpen>]
module Helper =
    let createRecord i =
      { Int = i
        String = string i
        Children = [ 
          { Int = i
            String = string i
            Children = [] }
        ] }

(*
|             Method |      N |   Type |            Mean |          Error |         StdDev |          Median | Ratio | RatioSD |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|------------------- |------- |------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|----------:|---------:|------:|----------:|
|             Option |     10 |    int |        125.5 ns |       1.527 ns |       1.429 ns |        126.0 ns |  1.00 |    0.00 |    0.1855 |        - |     - |     584 B |
|          SeqOption |     10 |    int |        382.8 ns |       2.818 ns |       2.498 ns |        382.4 ns |  3.04 |    0.03 |    0.2542 |        - |     - |     800 B |
|      CisternOption |     10 |    int |        353.0 ns |       2.493 ns |       2.210 ns |        353.6 ns |  2.81 |    0.03 |    0.2894 |        - |     - |     912 B |
| CisternValueOption |     10 |    int |        363.4 ns |       3.766 ns |       3.523 ns |        364.5 ns |  2.90 |    0.04 |    0.2131 |        - |     - |     672 B |
|                    |        |        |                 |                |                |                 |       |         |           |          |       |           |
|             Option |     10 | record |        208.2 ns |       1.858 ns |       1.647 ns |        208.1 ns |  1.00 |    0.00 |    0.1855 |        - |     - |     584 B |
|          SeqOption |     10 | record |        481.5 ns |       6.922 ns |       6.475 ns |        479.6 ns |  2.31 |    0.04 |    0.2537 |        - |     - |     800 B |
|      CisternOption |     10 | record |        633.3 ns |       8.346 ns |       7.807 ns |        636.0 ns |  3.04 |    0.05 |    0.3042 |        - |     - |     960 B |
| CisternValueOption |     10 | record |        654.5 ns |      21.642 ns |      28.891 ns |        646.6 ns |  3.17 |    0.18 |    0.2279 |        - |     - |     720 B |
|                    |        |        |                 |                |                |                 |       |         |           |          |       |           |
|             Option |   1000 |    int |     12,335.8 ns |      73.418 ns |      61.307 ns |     12,350.9 ns |  1.00 |    0.00 |   17.8070 |        - |     - |   56024 B |
|          SeqOption |   1000 |    int |     30,971.7 ns |     729.073 ns |     997.963 ns |     30,805.9 ns |  2.55 |    0.11 |   17.8223 |        - |     - |   56240 B |
|      CisternOption |   1000 |    int |     20,193.2 ns |     885.081 ns |   2,595.789 ns |     18,741.7 ns |  1.67 |    0.28 |   19.2871 |        - |     - |   60720 B |
| CisternValueOption |   1000 |    int |     19,656.6 ns |     263.131 ns |     246.133 ns |     19,662.7 ns |  1.59 |    0.02 |   11.6577 |        - |     - |   36720 B |
|                    |        |        |                 |                |                |                 |       |         |           |          |       |           |
|             Option |   1000 | record |     19,897.3 ns |     316.501 ns |     296.056 ns |     19,916.1 ns |  1.00 |    0.00 |   17.7917 |        - |     - |   56024 B |
|          SeqOption |   1000 | record |     38,961.6 ns |     419.313 ns |     392.225 ns |     39,125.2 ns |  1.96 |    0.04 |   17.8223 |        - |     - |   56240 B |
|      CisternOption |   1000 | record |     36,562.6 ns |   1,684.188 ns |   1,654.097 ns |     35,979.5 ns |  1.84 |    0.10 |   20.5688 |        - |     - |   64800 B |
| CisternValueOption |   1000 | record |     33,578.9 ns |     323.300 ns |     302.415 ns |     33,637.9 ns |  1.69 |    0.03 |   12.9395 |        - |     - |   40800 B |
|                    |        |        |                 |                |                |                 |       |         |           |          |       |           |
|             Option | 100000 |    int |  6,568,030.9 ns | 128,427.827 ns | 188,247.844 ns |  6,545,280.1 ns |  1.00 |    0.00 |  890.6250 | 445.3125 |     - | 5600024 B |
|          SeqOption | 100000 |    int |  8,302,556.5 ns | 116,593.369 ns | 109,061.510 ns |  8,356,214.1 ns |  1.26 |    0.04 |  890.6250 | 437.5000 |     - | 5600240 B |
|      CisternOption | 100000 |    int |  5,365,976.2 ns |  83,631.628 ns |  78,229.076 ns |  5,371,453.9 ns |  0.82 |    0.03 | 1000.0000 | 468.7500 |     - | 6014624 B |
| CisternValueOption | 100000 |    int |  3,707,578.3 ns |  58,202.269 ns |  54,442.438 ns |  3,685,188.3 ns |  0.56 |    0.02 |  578.1250 | 289.0625 |     - | 3614624 B |
|                    |        |        |                 |                |                |                 |       |         |           |          |       |           |
|             Option | 100000 | record |  9,756,506.7 ns | 192,917.107 ns | 483,991.300 ns |  9,811,197.7 ns |  1.00 |    0.00 |  890.6250 | 437.5000 |     - | 5600024 B |
|          SeqOption | 100000 | record | 12,209,906.6 ns | 242,517.822 ns | 630,336.064 ns | 12,218,609.4 ns |  1.25 |    0.09 |  890.6250 | 437.5000 |     - | 5600240 B |
|      CisternOption | 100000 | record |  8,466,737.5 ns | 160,275.275 ns | 320,087.043 ns |  8,507,475.0 ns |  0.87 |    0.06 | 1031.2500 | 515.6250 |     - | 6427376 B |
| CisternValueOption | 100000 | record |  7,453,745.3 ns | 222,802.617 ns | 656,938.543 ns |  7,547,209.0 ns |  0.77 |    0.08 |  640.6250 | 320.3125 |     - | 4027376 B |
*)
[<MemoryDiagnoser>]
type List_choose() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int list
    [<DefaultValue>] val mutable recs : Record list

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [1 .. this.N]
      this.recs <- List.init this.N createRecord

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> List.choose (fun x -> Some x) |> ignore
      | "record" -> this.recs |> List.choose (fun x -> Some x) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.SeqOption () =
      match this.Type with
      | "int" -> this.ints |> Seq.ofList |> Seq.choose (fun x -> Some x) |> Seq.toList |> ignore
      | "record" -> this.recs |> Seq.ofList |> Seq.choose (fun x -> Some x) |> Seq.toList |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.CisternOption () =
      match this.Type with
      | "int" -> this.ints |> Linq.ofList |> Linq.choose (fun x -> Some x) |> Linq.toList |> ignore
      | "record" -> this.recs |> Linq.ofList |> Linq.choose (fun x -> Some x) |> Linq.toList |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.CisternValueOption () =
      match this.Type with
      | "int" -> this.ints |> Linq.ofList |> Linq.chooseV (fun x -> ValueSome x) |> Linq.toList |> ignore
      | "record" -> this.recs |> Linq.ofList |> Linq.chooseV (fun x -> ValueSome x) |> Linq.toList |> ignore
      | _ -> failwith "Should never happen"

#if FSharpCoreValueOptionSupport
    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> List.chooseV (fun x -> ValueSome x) |> ignore
      | "record" -> this.recs |> List.chooseV (fun x -> ValueSome x) |> ignore
      | _ -> failwith "Should never happen"
#endif

(*
[<MemoryDiagnoser>]
type List_tryPick() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int list
    [<DefaultValue>] val mutable recs : Record list

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [1 .. this.N]
      this.recs <- List.init this.N createRecord

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> List.tryPick (fun x -> None) |> ignore
      | "record" -> this.recs |> List.tryPick (fun x -> None) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> List.tryPickV (fun x -> ValueNone) |> ignore
      | "record" -> this.recs |> List.tryPickV (fun x -> ValueNone) |> ignore
      | _ -> failwith "Should never happen"


[<MemoryDiagnoser>]
type List_unfoldV() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> 
          List.unfold (fun i -> if i > this.N then None else Some (i, (i + 1))) 0
          |> ignore
      | "record" -> 
          List.unfold (fun i -> if i > this.N then None else Some (createRecord i, (i + 1))) 0
          |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> 
          List.unfoldV (fun i -> if i > this.N then ValueNone else ValueSome (i, (i + 1))) 0
          |> ignore
      | "record" -> 
          List.unfoldV (fun i -> if i > this.N then ValueNone else ValueSome (createRecord i, (i + 1))) 0
          |> ignore
      | _ -> failwith "Should never happen"


[<MemoryDiagnoser>]
type Array_choose() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int array
    [<DefaultValue>] val mutable recs : Record array

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [|1 .. this.N|]
      this.recs <- Array.init this.N createRecord

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> Array.choose (fun x -> Some x) |> ignore
      | "record" -> this.recs |> Array.choose (fun x -> Some x) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> Array.chooseV (fun x -> ValueSome x) |> ignore
      | "record" -> this.recs |> Array.chooseV (fun x -> ValueSome x) |> ignore
      | _ -> failwith "Should never happen"


[<MemoryDiagnoser>]
type Array_tryPick() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int array
    [<DefaultValue>] val mutable recs : Record array

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [|1 .. this.N|]
      this.recs <- Array.init this.N createRecord

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> Array.tryPick (fun x -> None) |> ignore
      | "record" -> this.recs |> Array.tryPick (fun x -> None) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> Array.tryPickV (fun x -> ValueNone) |> ignore
      | "record" -> this.recs |> Array.tryPickV (fun x -> ValueNone) |> ignore
      | _ -> failwith "Should never happen"


[<MemoryDiagnoser>]
type Array_unfoldV() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> 
          Array.unfold (fun i -> if i > this.N then None else Some (i, (i + 1))) 0
          |> ignore
      | "record" -> 
          Array.unfold (fun i -> if i > this.N then None else Some (createRecord i, (i + 1))) 0
          |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> 
          Array.unfoldV (fun i -> if i > this.N then ValueNone else ValueSome (i, (i + 1))) 0
          |> ignore
      | "record" -> 
          Array.unfoldV (fun i -> if i > this.N then ValueNone else ValueSome (createRecord i, (i + 1))) 0
          |> ignore
      | _ -> failwith "Should never happen"


[<EntryPoint>]
let main argv =
    let summaries = BenchmarkRunner.Run(typeof<List_choose>.Assembly)
    printfn "%A" summaries
    0
*)