namespace Cistern.Linq.Benchmarking.FSharp.String

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp
open System

[<AbstractClass>]
type StringsBenchmarkBase () =
    let source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
    let filename = "lotsofwords.txt";

    [<Params(true, false)>]
    member val Sorted = false with get, set

    [<Params(0, 1, 10, 1000, 466544)>]
    member val WordsCount = 0 with get, set

    member val Words = [||] with get, set

    [<GlobalSetup>]
    member this.Setup() =
        if not (System.IO.File.Exists filename) then
            (new System.Net.WebClient()).DownloadFile (source, filename)

        this.Words <- System.IO.File.ReadAllLines filename

        if this.Sorted then
            this.Words <- this.Words |> Seq.sortBy (fun x -> x) |> Seq.toArray
        else
            // deterministric "random" shuffle...
            let r = Random 42
            for i = 0 to this.Words.Length-1 do
                let j = r.Next(i, this.Words.Length - 1)
                let tmp = this.Words.[i]
                this.Words.[i] <- this.Words.[j]
                this.Words.[j] <- tmp

        this.Words <- this.Words |> Seq.truncate this.WordsCount |> Seq.toArray

(*
|      Method | Sorted | WordsCount |             Mean |             Error |            StdDev | Ratio |
|------------ |------- |----------- |-----------------:|------------------:|------------------:|------:|
|   FSharpSeq |  False |          0 |         371.5 ns |         1.7361 ns |         1.6240 ns |  1.00 |
| CisternLinq |  False |          0 |         463.4 ns |         0.9876 ns |         0.9238 ns |  1.25 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |  False |          1 |         739.5 ns |         1.7000 ns |         1.4196 ns |  1.00 |
| CisternLinq |  False |          1 |         639.7 ns |         2.7706 ns |         2.5917 ns |  0.86 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |  False |         10 |       4,708.2 ns |        13.9996 ns |        13.0952 ns |  1.00 |
| CisternLinq |  False |         10 |       1,558.1 ns |         7.3960 ns |         6.9182 ns |  0.33 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |  False |       1000 |     467,011.5 ns |     4,771.3183 ns |     4,463.0941 ns |  1.00 |
| CisternLinq |  False |       1000 |     124,777.0 ns |     1,117.5742 ns |     1,045.3796 ns |  0.27 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |  False |     466544 | 217,686,731.1 ns | 1,771,457.5274 ns | 1,657,022.4702 ns |  1.00 |
| CisternLinq |  False |     466544 |  56,379,337.8 ns |   729,220.9990 ns |   682,113.7749 ns |  0.26 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |   True |          0 |         373.0 ns |         0.9073 ns |         0.8487 ns |  1.00 |
| CisternLinq |   True |          0 |         444.9 ns |         1.0573 ns |         0.9890 ns |  1.19 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |   True |          1 |         400.3 ns |         1.5658 ns |         1.3880 ns |  1.00 |
| CisternLinq |   True |          1 |         531.2 ns |         1.4335 ns |         1.3409 ns |  1.33 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |   True |         10 |       3,176.3 ns |         9.1483 ns |         8.5573 ns |  1.00 |
| CisternLinq |   True |         10 |       1,210.9 ns |         2.6064 ns |         2.1765 ns |  0.38 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |   True |       1000 |     319,389.3 ns |     1,173.3068 ns |     1,097.5119 ns |  1.00 |
| CisternLinq |   True |       1000 |      76,071.3 ns |       284.3833 ns |       266.0123 ns |  0.24 |
|             |        |            |                  |                   |                   |       |
|   FSharpSeq |   True |     466544 | 138,408,123.3 ns | 2,722,564.4068 ns | 2,546,688.4353 ns |  1.00 |
| CisternLinq |   True |     466544 |  30,112,740.8 ns |   441,881.3074 ns |   413,336.0491 ns |  0.22 |
*)
type String_GroupByCharCharChar() =
    inherit StringsBenchmarkBase()

    [<Benchmark(Baseline = true)>]
    member __.FSharpSeq () =
        base.Words
        |> Seq.filter (fun w -> w.Length >= 3)
        |> Seq.groupBy (fun w -> struct (w.[0], w.[1], w.[2]))
        |> dict

    [<Benchmark>]
    member __.CisternLinq() =
        base.Words
        |> Linq.filter (fun w -> w.Length >= 3)
        |> Linq.groupBy (fun w -> struct (w.[0], w.[1], w.[2]))
        |> Linq.dict
