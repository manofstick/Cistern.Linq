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