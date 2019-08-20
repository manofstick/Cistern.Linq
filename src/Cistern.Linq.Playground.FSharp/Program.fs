// Learn more about F# at http://fsharp.org

open Cistern.Linq.FSharp
open System.Diagnostics

[<EntryPoint>]
let main _ =
    Register.RegisterFSharpCollections ()

    let iterations = 1000000
    let repetitions = 3

    for _i = 1 to repetitions do
        let sw = Stopwatch.StartNew ()

        for i = 1 to iterations do
            let fibonacciLinq = Linq.unfold (fun (current, next) -> Some(current, (next, current + next))) (0, 1)
 
            let fibTotal =
                fibonacciLinq
                |> Linq.takeWhile (fun n -> n < 4000000)
                |> Linq.filter (fun n -> n % 2 = 0)
                |> Linq.sum

            burning_monk_euler.Helpers.validate 4613732 fibTotal

        printfn "Cistern %dms" sw.ElapsedMilliseconds

    for _i = 1 to repetitions do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            let fibonacciSeq = Seq.unfold (fun (current, next) -> Some(current, (next, current + next))) (0, 1)
 
            let fibTotal =
                fibonacciSeq
                |> Seq.takeWhile (fun n -> n < 4000000)
                |> Seq.filter (fun n -> n % 2 = 0)
                |> Seq.sum

            burning_monk_euler.Helpers.validate 4613732 fibTotal

        printfn "Seq %dms" sw.ElapsedMilliseconds

    0 // return an integer exit code
