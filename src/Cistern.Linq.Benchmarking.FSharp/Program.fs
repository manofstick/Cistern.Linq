module Cistern.Linq.Benchmarking.FSharp.Program

type GetAssembly = interface end

open BenchmarkDotNet.Running

[<EntryPoint>]
let main argv =
    BenchmarkSwitcher.FromAssembly typeof<GetAssembly>.Assembly
    |> fun switcher -> switcher.Run argv
    |> ignore

    0 // return an integer exit code
