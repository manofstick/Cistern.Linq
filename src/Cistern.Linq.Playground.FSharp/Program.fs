// Learn more about F# at http://fsharp.org

open System
open Cistern.Linq.FSharp
open System.Diagnostics

[<EntryPoint>]
let main argv =
    Register.RegisterFSharpCollections ()

    let listSize = 10000
    let iterations = 1
    let repetitions = 5

    let x = Random 42
    let test = List.init listSize (fun n -> x.NextDouble ())

    for _i = 1 to repetitions do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            let triangleNumber(n:int64) = [1L..n] |> Seq.sum
 
            let findFactorsOf(n:int64) =
                let upperBound = int64(Math.Sqrt(double(n)))
                [1L..upperBound] 
                |> Seq.filter (fun x -> n % x = 0L) 
                |> Seq.collect (fun x -> [x; n/x])
 
            let naturalNumbers = Seq.unfold (fun x -> Some(x, x+1L)) 1L
 
            let answer =
                naturalNumbers
                |> Seq.map (fun x -> triangleNumber(x))
                |> Seq.filter (fun x -> Seq.length(findFactorsOf(x)) >= 500)
                |> Seq.head

            total <- total  + float answer

        printfn "Seq %d (%f)" sw.ElapsedMilliseconds total

    for _i = 1 to repetitions do
        let sw = Stopwatch.StartNew ()

        let mutable total = 0.
        for i = 1 to iterations do
            let triangleNumber(n:int64) = [1L..n] |> Linq.sum
 
            let findFactorsOf(n:int64) =
                let upperBound = int64(Math.Sqrt(double(n)))
                [1L..upperBound] 
                |> Linq.filter (fun x -> n % x = 0L) 
                |> Linq.collect (fun x -> upcast [x; n/x])
 
            let naturalNumbers = Linq.unfold (fun x -> Some(x, x+1L)) 1L
 
            let answer =
                naturalNumbers
                |> Linq.map (fun x -> triangleNumber(x))
                |> Linq.filter (fun x -> Linq.length(findFactorsOf(x)) >= 500)
                |> Linq.head

            total <- total  + float answer

        printfn "Cistern %d (%f)" sw.ElapsedMilliseconds total



    0 // return an integer exit code
