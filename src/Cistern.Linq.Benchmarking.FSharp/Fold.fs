namespace Cistern.Linq.Benchmarking.FSharp.Fold

open BenchmarkDotNet.Attributes
open Cistern.Linq.FSharp

(*
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

type Fold_List () =
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

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |     74.23 ns |   0.8206 ns |   0.7675 ns |  1.00 |    0.00 |
|    List |             0 |     11.06 ns |   0.1260 ns |   0.1178 ns |  0.15 |    0.00 |
| Cistern |             0 |    104.27 ns |   0.9183 ns |   0.7668 ns |  1.40 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     92.71 ns |   1.1575 ns |   1.0828 ns |  1.00 |    0.00 |
|    List |             1 |     23.63 ns |   0.2443 ns |   0.2285 ns |  0.25 |    0.00 |
| Cistern |             1 |    123.52 ns |   1.8154 ns |   1.6982 ns |  1.33 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    281.18 ns |   3.8264 ns |   3.5792 ns |  1.00 |    0.00 |
|    List |            10 |    118.49 ns |   0.9054 ns |   0.8469 ns |  0.42 |    0.01 |
| Cistern |            10 |    175.84 ns |   1.7065 ns |   1.5962 ns |  0.63 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 18,832.09 ns | 236.5104 ns | 221.2320 ns |  1.00 |    0.00 |
|    List |          1000 | 10,318.20 ns | 205.1988 ns | 210.7241 ns |  0.55 |    0.01 |
| Cistern |          1000 |  4,736.75 ns |  52.1180 ns |  48.7512 ns |  0.25 |    0.00 |
*)
type Fold_ListMap () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Data : list<float> = [] with get, set

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- List.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data |> Seq.map id) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.List () = (0.0, this.Data |> List.map id) ||> List.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data |> Linq.map id) ||> Linq.fold (+) 

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |     73.17 ns |   0.6795 ns |   0.6356 ns |  1.00 |    0.00 |
|    List |             0 |     12.35 ns |   0.1324 ns |   0.1238 ns |  0.17 |    0.00 |
| Cistern |             0 |     92.51 ns |   1.1251 ns |   1.0524 ns |  1.26 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     83.32 ns |   0.6898 ns |   0.6452 ns |  1.00 |    0.00 |
|    List |             1 |     14.70 ns |   0.2031 ns |   0.1899 ns |  0.18 |    0.00 |
| Cistern |             1 |    121.56 ns |   1.2661 ns |   1.1843 ns |  1.46 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    228.66 ns |   1.6037 ns |   1.5001 ns |  1.00 |    0.00 |
|    List |            10 |     83.42 ns |   0.8975 ns |   0.8396 ns |  0.36 |    0.00 |
| Cistern |            10 |    156.63 ns |   1.3859 ns |   1.2964 ns |  0.69 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 13,704.15 ns |  97.7676 ns |  91.4519 ns |  1.00 |    0.00 |
|    List |          1000 |  6,374.14 ns | 117.3689 ns | 109.7869 ns |  0.47 |    0.01 |
| Cistern |          1000 |  4,119.63 ns |  53.5415 ns |  50.0827 ns |  0.30 |    0.00 |
*)
type Fold_ListFilter () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Data : list<float> = [] with get, set

    member this.Filter (x:float) =  (int x) < this.NumberOfItems/2

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- List.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data |> Seq.filter this.Filter) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.List () = (0.0, this.Data |> List.filter this.Filter) ||> List.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data |> Linq.filter this.Filter) ||> Linq.fold (+) 

(*
|  Method | NumberOfItems |         Mean |      Error |     StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|-----------:|-----------:|------:|--------:|
|     Seq |             0 |    14.904 ns |  0.1630 ns |  0.1524 ns |  1.00 |    0.00 |
|   Array |             0 |     8.726 ns |  0.0991 ns |  0.0927 ns |  0.59 |    0.01 |
| Cistern |             0 |    50.864 ns |  0.3691 ns |  0.3452 ns |  3.41 |    0.04 |
|         |               |              |            |            |       |         |
|     Seq |             1 |    27.760 ns |  0.2573 ns |  0.2149 ns |  1.00 |    0.00 |
|   Array |             1 |     9.735 ns |  0.0811 ns |  0.0759 ns |  0.35 |    0.00 |
| Cistern |             1 |    57.885 ns |  0.6925 ns |  0.6477 ns |  2.09 |    0.03 |
|         |               |              |            |            |       |         |
|     Seq |            10 |    79.789 ns |  1.0522 ns |  0.9842 ns |  1.00 |    0.00 |
|   Array |            10 |    23.908 ns |  0.3400 ns |  0.3181 ns |  0.30 |    0.00 |
| Cistern |            10 |    78.201 ns |  0.7378 ns |  0.6902 ns |  0.98 |    0.02 |
|         |               |              |            |            |       |         |
|     Seq |          1000 | 5,844.180 ns | 43.6971 ns | 40.8742 ns |  1.00 |    0.00 |
|   Array |          1000 | 1,541.301 ns | 13.3878 ns | 12.5229 ns |  0.26 |    0.00 |
| Cistern |          1000 | 2,098.673 ns | 25.2572 ns | 23.6256 ns |  0.36 |    0.00 |
*)
type Fold_Array () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Data : array<float> = [||] with get, set

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- Array.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.Array () = (0.0, this.Data) ||> Array.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data) ||> Linq.fold (+) 

(*
|  Method | NumberOfItems |         Mean |       Error |      StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|------------:|------:|--------:|
|     Seq |             0 |     64.68 ns |   1.0814 ns |   1.0116 ns |  1.00 |    0.00 |
|   Array |             0 |     13.83 ns |   0.3032 ns |   0.2978 ns |  0.21 |    0.01 |
| Cistern |             0 |     80.72 ns |   1.2066 ns |   1.1287 ns |  1.25 |    0.03 |
|         |               |              |             |             |       |         |
|     Seq |             1 |     88.40 ns |   1.1646 ns |   1.0893 ns |  1.00 |    0.00 |
|   Array |             1 |     15.16 ns |   0.3004 ns |   0.2810 ns |  0.17 |    0.00 |
| Cistern |             1 |    105.31 ns |   1.3021 ns |   1.2180 ns |  1.19 |    0.02 |
|         |               |              |             |             |       |         |
|     Seq |            10 |    256.67 ns |   5.1103 ns |   5.0190 ns |  1.00 |    0.00 |
|   Array |            10 |     42.91 ns |   0.8688 ns |   0.8127 ns |  0.17 |    0.00 |
| Cistern |            10 |    140.01 ns |   2.6019 ns |   2.4338 ns |  0.55 |    0.01 |
|         |               |              |             |             |       |         |
|     Seq |          1000 | 15,545.35 ns | 273.8500 ns | 256.1595 ns |  1.00 |    0.00 |
|   Array |          1000 |  2,790.93 ns |  24.0421 ns |  20.0762 ns |  0.18 |    0.00 |
| Cistern |          1000 |  4,292.14 ns |  70.5940 ns |  66.0337 ns |  0.28 |    0.00 |
*)
type Fold_ArrayMap () =
    [<Params(0, 1, 10, 1000)>]
    member val NumberOfItems = 0 with get, set

    member val Data : array<float> = [||] with get, set

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- Array.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data |> Seq.map id) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.Array () = (0.0, this.Data |> Array.map id) ||> Array.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data |> Linq.map id) ||> Linq.fold (+) 

(*
|  Method | NumberOfItems |         Mean |       Error |     StdDev | Ratio | RatioSD |
|-------- |-------------- |-------------:|------------:|-----------:|------:|--------:|
|     Seq |             0 |     63.24 ns |   0.7929 ns |  0.7416 ns |  1.00 |    0.00 |
|   Array |             0 |     25.28 ns |   0.4606 ns |  0.4308 ns |  0.40 |    0.01 |
| Cistern |             0 |     84.04 ns |   0.8212 ns |  0.6858 ns |  1.33 |    0.03 |
|         |               |              |             |            |       |         |
|     Seq |             1 |     80.68 ns |   0.8383 ns |  0.7841 ns |  1.00 |    0.00 |
|   Array |             1 |     30.17 ns |   0.3648 ns |  0.3412 ns |  0.37 |    0.01 |
| Cistern |             1 |     97.17 ns |   0.7210 ns |  0.6744 ns |  1.20 |    0.01 |
|         |               |              |             |            |       |         |
|     Seq |            10 |    210.76 ns |   2.1418 ns |  2.0034 ns |  1.00 |    0.00 |
|   Array |            10 |     80.53 ns |   0.8848 ns |  0.8277 ns |  0.38 |    0.01 |
| Cistern |            10 |    130.55 ns |   1.6248 ns |  1.5198 ns |  0.62 |    0.01 |
|         |               |              |             |            |       |         |
|     Seq |          1000 | 12,136.52 ns | 102.5527 ns | 95.9279 ns |  1.00 |    0.00 |
|   Array |          1000 |  3,604.40 ns |  17.5946 ns | 16.4580 ns |  0.30 |    0.00 |
| Cistern |          1000 |  3,357.86 ns |  34.8379 ns | 32.5874 ns |  0.28 |    0.00 |
*)
type Fold_ArrayFilter () =
    [<Params(0, 1, 10, 1000)>]

    member val NumberOfItems = 0 with get, set

    member val Data : array<float> = [||] with get, set

    member this.Filter (x:float) =  (int x) < this.NumberOfItems/2

    [<GlobalSetup>]
    member this.Setup () =
        this.Data <- Array.init this.NumberOfItems float

    [<Benchmark (Baseline = true)>]
    member this.Seq () = (0.0, this.Data |> Seq.filter this.Filter) ||> Seq.fold (+) 

    [<Benchmark>]
    member this.Array () = (0.0, this.Data |> Array.filter this.Filter) ||> Array.fold (+) 

    [<Benchmark>]
    member this.Cistern () = (0.0, this.Data |> Linq.filter this.Filter) ||> Linq.fold (+) 
