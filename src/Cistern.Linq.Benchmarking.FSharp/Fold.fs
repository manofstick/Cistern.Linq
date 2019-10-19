namespace Cistern.Linq.Benchmarking.FSharp.Unfold

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |    22.243 ns |   0.2657 ns |   0.2485 ns |  1.00 |    0.00 |
|    List |             0 |     4.641 ns |   0.1092 ns |   0.1022 ns |  0.21 |    0.01 |
| Cistern |             0 |    56.290 ns |   0.9168 ns |   0.8576 ns |  2.53 |    0.05 |
|         |               |              |             |             |       |         |
|     Seq |             1 |    31.133 ns |   0.3076 ns |   0.2726 ns |  1.00 |    0.00 |
|    List |             1 |     9.408 ns |   0.1358 ns |   0.1271 ns |  0.30 |    0.01 |
| Cistern |             1 |    78.657 ns |   1.1108 ns |   1.0391 ns |  2.52 |    0.04 |
|         |               |              |             |             |       |         |
|     Seq |            10 |   122.263 ns |   1.5532 ns |   1.4529 ns |  1.00 |    0.00 |
|    List |            10 |    26.118 ns |   0.3990 ns |   0.3732 ns |  0.21 |    0.00 |
| Cistern |            10 |   106.650 ns |   2.1550 ns |   4.2031 ns |  0.89 |    0.03 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 8,510.427 ns | 115.3820 ns | 107.9284 ns |  1.00 |    0.00 |
|    List |          1000 | 1,847.191 ns |  22.2721 ns |  20.8333 ns |  0.22 |    0.00 |
| Cistern |          1000 | 2,770.954 ns |  36.9848 ns |  34.5956 ns |  0.33 |    0.01 |

|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |    22.567 ns |   0.3456 ns |   0.3063 ns |  1.00 |    0.00 |
|    List |             0 |     4.542 ns |   0.0881 ns |   0.0824 ns |  0.20 |    0.01 |
| Cistern |             0 |    55.802 ns |   0.7417 ns |   0.6937 ns |  2.48 |    0.04 |
|         |               |              |             |             |       |         |
|     Seq |             1 |    30.818 ns |   0.5089 ns |   0.4760 ns |  1.00 |    0.00 |
|    List |             1 |     9.309 ns |   0.1405 ns |   0.1314 ns |  0.30 |    0.01 |
| Cistern |             1 |    77.056 ns |   1.2164 ns |   1.1379 ns |  2.50 |    0.05 |
|         |               |              |             |             |       |         |
|     Seq |            10 |   121.398 ns |   1.5246 ns |   1.4261 ns |  1.00 |    0.00 |
|    List |            10 |    26.875 ns |   0.4353 ns |   0.4072 ns |  0.22 |    0.00 |
| Cistern |            10 |   100.600 ns |   1.1917 ns |   1.1147 ns |  0.83 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 8,412.862 ns | 124.2634 ns | 116.2360 ns |  1.00 |    0.00 |
|    List |          1000 | 1,847.420 ns |  22.7775 ns |  21.3061 ns |  0.22 |    0.00 |
| Cistern |          1000 | 2,666.350 ns |  63.6237 ns |  56.4008 ns |  0.32 |    0.01 |
*)

type Fold () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Data : list<float> = [] with get, set

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- List.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.List () = (0.0, this.Data) ||> List.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data) ||> Linq.fold (+) 
