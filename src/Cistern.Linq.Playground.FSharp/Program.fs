// Learn more about F# at http://fsharp.org

open System
open Cistern.Linq.FSharp
open System.Diagnostics

[<EntryPoint>]
let main argv =
    Register.RegisterFSharpCollections ()

    let listSize = 10000
    let iterations = 10000

    let x = Random 42
    let test = List.init listSize (fun n -> x.NextDouble ())

    for _i = 1 to 5 do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            total <- 
                test
                |> Linq.map (fun x -> x * x)
                |> Linq.sum

        printfn "Cistern %d (%f)" sw.ElapsedMilliseconds total

    for _i = 1 to 5 do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            total <- 
                test
                |> List.map (fun x -> x * x)
                |> List.sum

        printfn "List %d (%f)" sw.ElapsedMilliseconds total

    for _i = 1 to 5 do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            total <- 
                test
                |> Seq.map (fun x -> x * x)
                |> Seq.sum

        printfn "Seq %d (%f)" sw.ElapsedMilliseconds total


    0 // return an integer exit code
