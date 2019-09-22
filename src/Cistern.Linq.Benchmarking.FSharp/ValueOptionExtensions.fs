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
|             Method |      N |   Type |            Mean |          Error |         StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 | Gen 2 | Allocated |
|------------------- |------- |------- |----------------:|---------------:|---------------:|------:|--------:|---------:|---------:|------:|----------:|
|             Option |     10 |    int |        131.2 ns |       1.651 ns |       1.544 ns |  1.00 |    0.00 |   0.1855 |        - |     - |     584 B |
|          SeqOption |     10 |    int |        414.3 ns |       5.455 ns |       5.102 ns |  3.16 |    0.05 |   0.2542 |        - |     - |     800 B |
|      CisternOption |     10 |    int |        416.8 ns |       6.321 ns |       5.912 ns |  3.18 |    0.06 |   0.2489 |        - |     - |     784 B |
| CisternValueOption |     10 |    int |        378.9 ns |       4.956 ns |       4.636 ns |  2.89 |    0.04 |   0.1726 |        - |     - |     544 B |
|                    |        |        |                 |                |                |       |         |          |          |       |           |
|             Option |     10 | record |        215.2 ns |       2.124 ns |       1.987 ns |  1.00 |    0.00 |   0.1855 |        - |     - |     584 B |
|          SeqOption |     10 | record |        488.1 ns |       7.553 ns |       7.066 ns |  2.27 |    0.04 |   0.2537 |        - |     - |     800 B |
|      CisternOption |     10 | record |        561.3 ns |       7.852 ns |       7.345 ns |  2.61 |    0.04 |   0.2489 |        - |     - |     784 B |
| CisternValueOption |     10 | record |        543.7 ns |       9.064 ns |       8.035 ns |  2.53 |    0.04 |   0.1726 |        - |     - |     544 B |
|                    |        |        |                 |                |                |       |         |          |          |       |           |
|             Option |   1000 |    int |     12,815.7 ns |     162.921 ns |     152.397 ns |  1.00 |    0.00 |  17.8070 |        - |     - |   56024 B |
|          SeqOption |   1000 |    int |     31,890.5 ns |     399.589 ns |     373.776 ns |  2.49 |    0.05 |  17.8223 |        - |     - |   56240 B |
|      CisternOption |   1000 |    int |     24,479.5 ns |     255.382 ns |     238.885 ns |  1.91 |    0.03 |  17.8528 |        - |     - |   56224 B |
| CisternValueOption |   1000 |    int |     25,767.1 ns |     506.437 ns |     693.217 ns |  2.02 |    0.07 |  10.2234 |        - |     - |   32224 B |
|                    |        |        |                 |                |                |       |         |          |          |       |           |
|             Option |   1000 | record |     19,558.7 ns |     349.262 ns |     326.700 ns |  1.00 |    0.00 |  17.7917 |        - |     - |   56024 B |
|          SeqOption |   1000 | record |     39,661.0 ns |     549.528 ns |     487.142 ns |  2.03 |    0.04 |  17.8223 |        - |     - |   56240 B |
|      CisternOption |   1000 | record |     39,526.7 ns |     466.046 ns |     435.939 ns |  2.02 |    0.05 |  17.8223 |        - |     - |   56224 B |
| CisternValueOption |   1000 | record |     37,212.1 ns |     559.805 ns |     523.642 ns |  1.90 |    0.04 |  10.1929 |        - |     - |   32224 B |
|                    |        |        |                 |                |                |       |         |          |          |       |           |
|             Option | 100000 |    int |  6,581,031.5 ns | 119,568.304 ns | 111,844.266 ns |  1.00 |    0.00 | 890.6250 | 445.3125 |     - | 5600024 B |
|          SeqOption | 100000 |    int |  8,593,219.4 ns | 116,786.363 ns | 109,242.036 ns |  1.31 |    0.03 | 890.6250 | 437.5000 |     - | 5600240 B |
|      CisternOption | 100000 |    int |  7,884,429.6 ns | 152,840.971 ns | 156,956.424 ns |  1.20 |    0.03 | 890.6250 | 437.5000 |     - | 5600224 B |
| CisternValueOption | 100000 |    int |  4,411,870.4 ns |  63,043.326 ns |  58,970.766 ns |  0.67 |    0.01 | 515.6250 | 250.0000 |     - | 3200224 B |
|                    |        |        |                 |                |                |       |         |          |          |       |           |
|             Option | 100000 | record |  9,525,180.6 ns | 179,080.012 ns | 183,901.987 ns |  1.00 |    0.00 | 890.6250 | 437.5000 |     - | 5600024 B |
|          SeqOption | 100000 | record | 11,699,129.0 ns | 196,779.117 ns | 184,067.308 ns |  1.23 |    0.03 | 890.6250 | 437.5000 |     - | 5600240 B |
|      CisternOption | 100000 | record | 10,828,119.8 ns |  48,011.657 ns |  42,561.083 ns |  1.14 |    0.02 | 890.6250 | 437.5000 |     - | 5600224 B |
| CisternValueOption | 100000 | record |  6,780,783.3 ns | 141,351.855 ns | 211,568.800 ns |  0.72 |    0.03 | 515.6250 | 250.0000 |     - | 3200224 B |
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