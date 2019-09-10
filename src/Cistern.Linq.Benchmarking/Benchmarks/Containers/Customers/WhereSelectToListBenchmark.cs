using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<string>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |            Mean |          Error |         StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |--------------- |-------------- |----------------:|---------------:|---------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |     ForLoop |          Array |            10 |        249.0 ns |       2.013 ns |       1.883 ns |  0.59 |    0.00 |   0.0682 |       - |       - |     216 B |
    |  SystemLinq |          Array |            10 |        423.8 ns |       3.173 ns |       2.477 ns |  1.00 |    0.00 |   0.0911 |       - |       - |     288 B |
    | CisternLinq |          Array |            10 |        419.7 ns |       3.953 ns |       3.698 ns |  0.99 |    0.01 |   0.1268 |       - |       - |     400 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |          Array |          1000 |     26,884.2 ns |     145.343 ns |     135.954 ns |  0.99 |    0.01 |   5.2795 |       - |       - |   16640 B |
    |  SystemLinq |          Array |          1000 |     27,236.5 ns |     222.584 ns |     208.205 ns |  1.00 |    0.00 |   5.2795 |       - |       - |   16712 B |
    | CisternLinq |          Array |          1000 |     25,559.0 ns |     185.181 ns |     164.158 ns |  0.94 |    0.01 |   5.3406 |       - |       - |   16824 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |          Array |        100000 |  4,947,652.8 ns |  98,499.638 ns | 252,492.228 ns |  0.86 |    0.06 | 109.3750 | 70.3125 | 70.3125 | 2098140 B |
    |  SystemLinq |          Array |        100000 |  5,742,249.6 ns | 113,204.540 ns | 248,486.709 ns |  1.00 |    0.00 | 109.3750 | 70.3125 | 70.3125 | 2098203 B |
    | CisternLinq |          Array |        100000 |  5,212,204.3 ns | 103,731.154 ns | 304,225.520 ns |  0.90 |    0.07 | 101.5625 | 62.5000 | 62.5000 | 2098231 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |           List |            10 |        273.4 ns |       2.494 ns |       2.333 ns |  0.51 |    0.00 |   0.0710 |       - |       - |     224 B |
    |  SystemLinq |           List |            10 |        537.1 ns |       4.827 ns |       4.515 ns |  1.00 |    0.00 |   0.1059 |       - |       - |     336 B |
    | CisternLinq |           List |            10 |        557.4 ns |       2.682 ns |       2.377 ns |  1.04 |    0.01 |   0.1364 |       - |       - |     432 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |           List |          1000 |     30,876.2 ns |     284.871 ns |     266.468 ns |  0.83 |    0.01 |   5.2490 |       - |       - |   16648 B |
    |  SystemLinq |           List |          1000 |     37,090.6 ns |     276.905 ns |     259.017 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16760 B |
    | CisternLinq |           List |          1000 |     32,883.7 ns |     246.733 ns |     230.794 ns |  0.89 |    0.01 |   5.3101 |       - |       - |   16856 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |           List |        100000 |  5,364,577.0 ns | 106,614.715 ns | 309,308.621 ns |  0.83 |    0.05 | 109.3750 | 70.3125 | 70.3125 | 2098142 B |
    |  SystemLinq |           List |        100000 |  6,454,462.4 ns | 128,266.522 ns | 299,818.860 ns |  1.00 |    0.00 | 109.3750 | 70.3125 | 70.3125 | 2098180 B |
    | CisternLinq |           List |        100000 |  6,134,600.4 ns | 122,629.523 ns | 236,265.459 ns |  0.95 |    0.06 | 109.3750 | 70.3125 | 70.3125 | 2098336 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |            10 |        273.7 ns |       2.297 ns |       2.149 ns |  0.45 |    0.01 |   0.0787 |       - |       - |     248 B |
    |  SystemLinq |     Enumerable |            10 |        611.0 ns |       4.481 ns |       4.191 ns |  1.00 |    0.00 |   0.1163 |       - |       - |     368 B |
    | CisternLinq |     Enumerable |            10 |        602.4 ns |       4.134 ns |       3.665 ns |  0.99 |    0.01 |   0.1469 |       - |       - |     464 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |          1000 |     28,257.3 ns |     306.416 ns |     286.622 ns |  0.75 |    0.01 |   5.2795 |       - |       - |   16672 B |
    |  SystemLinq |     Enumerable |          1000 |     37,726.5 ns |     399.240 ns |     373.450 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16792 B |
    | CisternLinq |     Enumerable |          1000 |     33,231.2 ns |     361.907 ns |     338.528 ns |  0.88 |    0.01 |   5.3101 |       - |       - |   16891 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |        100000 |  4,851,224.6 ns | 101,063.567 ns | 223,949.884 ns |  0.71 |    0.05 | 109.3750 | 70.3125 | 70.3125 | 2098166 B |
    |  SystemLinq |     Enumerable |        100000 |  6,831,987.7 ns | 135,368.040 ns | 279,558.476 ns |  1.00 |    0.00 | 109.3750 | 70.3125 | 70.3125 | 2098269 B |
    | CisternLinq |     Enumerable |        100000 |  5,945,804.3 ns | 117,968.885 ns | 320,943.052 ns |  0.89 |    0.06 | 109.3750 | 70.3125 | 70.3125 | 2098427 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |            10 |        254.0 ns |       2.693 ns |       2.519 ns |  0.47 |    0.01 |   0.0682 |       - |       - |     216 B |
    |  SystemLinq | ImmutableArray |            10 |        543.5 ns |       4.271 ns |       3.786 ns |  1.00 |    0.00 |   0.1059 |       - |       - |     336 B |
    | CisternLinq | ImmutableArray |            10 |        520.6 ns |       4.071 ns |       3.808 ns |  0.96 |    0.01 |   0.1316 |       - |       - |     416 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |          1000 |     26,841.8 ns |     209.532 ns |     195.997 ns |  0.77 |    0.01 |   5.2795 |       - |       - |   16640 B |
    |  SystemLinq | ImmutableArray |          1000 |     34,948.3 ns |     288.371 ns |     269.743 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16760 B |
    | CisternLinq | ImmutableArray |          1000 |     25,121.4 ns |     103.036 ns |      96.380 ns |  0.72 |    0.01 |   5.3406 |       - |       - |   16842 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |        100000 |  4,926,120.6 ns |  97,589.576 ns | 267,149.889 ns |  0.77 |    0.06 | 101.5625 | 62.5000 | 62.5000 | 2098070 B |
    |  SystemLinq | ImmutableArray |        100000 |  6,342,157.1 ns | 126,538.690 ns | 207,906.696 ns |  1.00 |    0.00 | 109.3750 | 70.3125 | 70.3125 | 2098157 B |
    | CisternLinq | ImmutableArray |        100000 |  5,049,089.2 ns | 100,020.184 ns | 239,641.973 ns |  0.80 |    0.05 | 109.3750 | 70.3125 | 70.3125 | 2098352 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |            10 |      1,308.1 ns |       9.431 ns |       8.821 ns |  0.78 |    0.01 |   0.0801 |       - |       - |     256 B |
    |  SystemLinq |  ImmutableList |            10 |      1,673.6 ns |      11.209 ns |       9.937 ns |  1.00 |    0.00 |   0.1183 |       - |       - |     376 B |
    | CisternLinq |  ImmutableList |            10 |      1,717.9 ns |      12.267 ns |      11.474 ns |  1.03 |    0.01 |   0.1564 |       - |       - |     496 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |          1000 |    108,886.2 ns |     683.485 ns |     605.892 ns |  0.92 |    0.01 |   5.2490 |       - |       - |   16680 B |
    |  SystemLinq |  ImmutableList |          1000 |    119,054.4 ns |     967.288 ns |     904.801 ns |  1.00 |    0.00 |   5.2490 |       - |       - |   16800 B |
    | CisternLinq |  ImmutableList |          1000 |    114,074.2 ns |     898.635 ns |     840.584 ns |  0.96 |    0.01 |   5.3711 |       - |       - |   16922 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |        100000 | 14,256,602.0 ns | 287,289.022 ns | 472,024.102 ns |  0.92 |    0.04 |  93.7500 | 62.5000 | 62.5000 | 2097866 B |
    |  SystemLinq |  ImmutableList |        100000 | 15,512,583.2 ns | 305,777.760 ns | 300,314.646 ns |  1.00 |    0.00 |  93.7500 | 62.5000 | 62.5000 | 2097982 B |
    | CisternLinq |  ImmutableList |        100000 | 14,681,457.0 ns | 288,108.568 ns | 448,550.330 ns |  0.95 |    0.03 |  93.7500 | 62.5000 | 62.5000 | 2098010 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |            10 |        252.7 ns |       1.867 ns |       1.746 ns |  0.41 |    0.00 |   0.0710 |       - |       - |     224 B |
    |  SystemLinq |     FSharpList |            10 |        620.0 ns |       3.470 ns |       3.246 ns |  1.00 |    0.00 |   0.1087 |       - |       - |     344 B |
    | CisternLinq |     FSharpList |            10 |        646.7 ns |       4.736 ns |       4.430 ns |  1.04 |    0.01 |   0.1316 |       - |       - |     416 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |          1000 |     28,451.1 ns |     257.731 ns |     241.082 ns |  0.77 |    0.01 |   5.2795 |       - |       - |   16648 B |
    |  SystemLinq |     FSharpList |          1000 |     37,102.0 ns |     399.523 ns |     373.714 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16768 B |
    | CisternLinq |     FSharpList |          1000 |     32,280.5 ns |     316.721 ns |     296.261 ns |  0.87 |    0.01 |   5.3101 |       - |       - |   16842 B |
    |             |                |               |                 |                |                |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |        100000 |  6,238,661.3 ns | 123,434.244 ns | 222,577.402 ns |  0.92 |    0.05 | 101.5625 | 62.5000 | 62.5000 | 2098048 B |
    |  SystemLinq |     FSharpList |        100000 |  6,834,743.2 ns | 134,432.596 ns | 274,610.170 ns |  1.00 |    0.00 | 101.5625 | 62.5000 | 62.5000 | 2098184 B |
    | CisternLinq |     FSharpList |        100000 |  6,442,766.9 ns | 125,677.939 ns | 226,623.248 ns |  0.95 |    0.06 | 101.5625 | 62.5000 | 62.5000 | 2098216 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_SelectWhereToListBenchmark : CustomersBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var subset = new List<string>();
            foreach (var c in Customers)
            {
                if (c.DOB.Year < 1980)
                {
                    subset.Add(c.Name);
                }
            }
            return subset;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.Select(
                        System.Linq.Enumerable.Where(
                            Customers,
                            c => c.DOB.Year < 1980),
                        c => c.Name)
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return 
                Customers
                .Where(c => c.DOB.Year < 1980)
                .Select(c => c.Name)
                .ToList();
        }

    }
}
