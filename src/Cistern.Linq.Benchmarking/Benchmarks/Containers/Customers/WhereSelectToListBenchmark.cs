using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<string>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |            Mean |          Error |         StdDev |          Median | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
    |------------ |--------------- |-------------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|---------:|--------:|--------:|----------:|
    |     ForLoop |          Array |            10 |        207.3 ns |       2.697 ns |       2.523 ns |        207.3 ns |  0.76 |    0.02 |   0.0660 |       - |       - |     208 B |
    |  SystemLinq |          Array |            10 |        272.6 ns |       4.279 ns |       4.003 ns |        273.1 ns |  1.00 |    0.00 |   0.0892 |       - |       - |     280 B |
    | CisternLinq |          Array |            10 |        335.0 ns |       3.837 ns |       3.590 ns |        334.8 ns |  1.23 |    0.02 |   0.0968 |       - |       - |     304 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |          Array |          1000 |     23,267.4 ns |     249.711 ns |     233.580 ns |     23,247.4 ns |  1.09 |    0.01 |   5.2795 |       - |       - |   16632 B |
    |  SystemLinq |          Array |          1000 |     21,340.8 ns |     170.429 ns |     151.081 ns |     21,389.8 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16704 B |
    | CisternLinq |          Array |          1000 |     21,687.7 ns |     265.303 ns |     248.165 ns |     21,698.6 ns |  1.01 |    0.01 |   5.3101 |       - |       - |   16728 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |          Array |        100000 |  3,660,599.2 ns |  75,331.429 ns | 219,745.314 ns |  3,552,787.3 ns |  1.05 |    0.09 | 101.5625 | 62.5000 | 62.5000 |  131852 B |
    |  SystemLinq |          Array |        100000 |  3,511,782.7 ns | 169,711.293 ns | 214,630.447 ns |  3,412,160.9 ns |  1.00 |    0.00 | 105.4688 | 66.4063 | 66.4063 |  131931 B |
    | CisternLinq |          Array |        100000 |  3,510,327.1 ns |  92,115.607 ns |  90,469.843 ns |  3,501,145.7 ns |  0.99 |    0.07 | 101.5625 | 62.5000 | 62.5000 |  131929 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |           List |            10 |        257.4 ns |       3.255 ns |       3.044 ns |        258.2 ns |  0.89 |    0.02 |   0.0687 |       - |       - |     216 B |
    |  SystemLinq |           List |            10 |        290.2 ns |       4.722 ns |       4.417 ns |        289.9 ns |  1.00 |    0.00 |   0.1044 |       - |       - |     328 B |
    | CisternLinq |           List |            10 |        453.5 ns |       7.940 ns |       7.427 ns |        453.0 ns |  1.56 |    0.04 |   0.1121 |       - |       - |     352 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |           List |          1000 |     26,809.6 ns |     456.695 ns |     427.193 ns |     26,687.8 ns |  1.20 |    0.02 |   5.2795 |       - |       - |   16640 B |
    |  SystemLinq |           List |          1000 |     22,263.2 ns |     193.494 ns |     171.527 ns |     22,209.3 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16752 B |
    | CisternLinq |           List |          1000 |     29,788.4 ns |     471.637 ns |     441.170 ns |     29,781.7 ns |  1.34 |    0.02 |   5.3406 |       - |       - |   16776 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |           List |        100000 |  4,281,788.6 ns | 111,936.875 ns | 330,048.402 ns |  4,279,545.5 ns |  1.04 |    0.15 | 109.3750 | 70.3125 | 70.3125 |  131880 B |
    |  SystemLinq |           List |        100000 |  4,163,727.0 ns | 156,162.412 ns | 460,448.394 ns |  4,175,455.9 ns |  1.00 |    0.00 | 101.5625 | 62.5000 | 62.5000 |  131979 B |
    | CisternLinq |           List |        100000 |  4,312,753.7 ns |  85,205.441 ns | 213,763.792 ns |  4,265,797.7 ns |  1.05 |    0.12 | 101.5625 | 62.5000 | 62.5000 |  132004 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |            10 |        236.2 ns |       4.455 ns |       4.167 ns |        237.7 ns |  0.64 |    0.01 |   0.0758 |       - |       - |     240 B |
    |  SystemLinq |     Enumerable |            10 |        368.5 ns |       5.061 ns |       4.734 ns |        366.7 ns |  1.00 |    0.00 |   0.1144 |       - |       - |     360 B |
    | CisternLinq |     Enumerable |            10 |        478.2 ns |       6.166 ns |       5.768 ns |        479.8 ns |  1.30 |    0.02 |   0.1221 |       - |       - |     384 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |          1000 |     25,305.6 ns |     280.806 ns |     262.666 ns |     25,377.9 ns |  0.89 |    0.01 |   5.2795 |       - |       - |   16664 B |
    |  SystemLinq |     Enumerable |          1000 |     28,501.9 ns |     142.925 ns |     133.692 ns |     28,539.4 ns |  1.00 |    0.00 |   5.3406 |       - |       - |   16784 B |
    | CisternLinq |     Enumerable |          1000 |     28,554.5 ns |     437.029 ns |     408.798 ns |     28,515.9 ns |  1.00 |    0.02 |   5.3406 |       - |       - |   16811 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     Enumerable |        100000 |  3,902,128.9 ns | 115,072.038 ns | 339,292.502 ns |  3,771,228.3 ns |  0.92 |    0.10 | 105.4688 | 66.4063 | 66.4063 |  131914 B |
    |  SystemLinq |     Enumerable |        100000 |  4,268,367.4 ns |  97,586.560 ns | 284,664.576 ns |  4,187,016.4 ns |  1.00 |    0.00 | 101.5625 | 62.5000 | 62.5000 |  132008 B |
    | CisternLinq |     Enumerable |        100000 |  4,389,276.1 ns | 109,902.490 ns | 318,847.052 ns |  4,307,417.2 ns |  1.03 |    0.10 | 101.5625 | 62.5000 | 62.5000 |  132050 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |            10 |        216.1 ns |       3.988 ns |       3.730 ns |        214.4 ns |  0.64 |    0.02 |   0.0663 |       - |       - |     208 B |
    |  SystemLinq | ImmutableArray |            10 |        339.4 ns |       4.309 ns |       4.030 ns |        340.3 ns |  1.00 |    0.00 |   0.1044 |       - |       - |     328 B |
    | CisternLinq | ImmutableArray |            10 |        427.8 ns |       6.333 ns |       5.924 ns |        430.3 ns |  1.26 |    0.01 |   0.1068 |       - |       - |     336 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |          1000 |     23,506.3 ns |     319.792 ns |     299.134 ns |     23,408.8 ns |  0.90 |    0.02 |   5.2795 |       - |       - |   16632 B |
    |  SystemLinq | ImmutableArray |          1000 |     26,033.5 ns |     346.564 ns |     324.176 ns |     26,073.2 ns |  1.00 |    0.00 |   5.3101 |       - |       - |   16752 B |
    | CisternLinq | ImmutableArray |          1000 |     22,007.9 ns |     389.517 ns |     364.355 ns |     21,956.5 ns |  0.85 |    0.02 |   5.3406 |       - |       - |   16762 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop | ImmutableArray |        100000 |  3,746,552.0 ns | 121,561.691 ns | 358,427.388 ns |  3,513,137.1 ns |  0.97 |    0.08 | 105.4688 | 66.4063 | 66.4063 |  131901 B |
    |  SystemLinq | ImmutableArray |        100000 |  3,874,208.2 ns |  85,418.751 ns | 116,922.150 ns |  3,861,939.5 ns |  1.00 |    0.00 | 109.3750 | 70.3125 | 70.3125 |  131988 B |
    | CisternLinq | ImmutableArray |        100000 |  3,702,982.5 ns | 130,143.237 ns | 383,730.271 ns |  3,442,799.8 ns |  0.97 |    0.10 | 101.5625 | 62.5000 | 62.5000 |  131978 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |            10 |      1,225.5 ns |      20.145 ns |      18.843 ns |      1,228.2 ns |  0.92 |    0.02 |   0.0763 |       - |       - |     248 B |
    |  SystemLinq |  ImmutableList |            10 |      1,336.4 ns |      18.021 ns |      16.856 ns |      1,341.1 ns |  1.00 |    0.00 |   0.1163 |       - |       - |     368 B |
    | CisternLinq |  ImmutableList |            10 |      1,487.3 ns |      18.351 ns |      17.165 ns |      1,482.3 ns |  1.11 |    0.02 |   0.1316 |       - |       - |     416 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |          1000 |    103,049.4 ns |   1,482.299 ns |   1,386.543 ns |    103,416.8 ns |  0.98 |    0.02 |   5.2490 |       - |       - |   16672 B |
    |  SystemLinq |  ImmutableList |          1000 |    104,984.7 ns |     777.615 ns |     689.335 ns |    105,120.3 ns |  1.00 |    0.00 |   5.2490 |       - |       - |   16792 B |
    | CisternLinq |  ImmutableList |          1000 |    104,459.6 ns |   1,157.807 ns |   1,083.013 ns |    104,435.1 ns |  0.99 |    0.01 |   5.2490 |       - |       - |   16842 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |  ImmutableList |        100000 | 13,268,102.8 ns | 264,083.842 ns | 740,519.406 ns | 13,441,646.9 ns |  0.99 |    0.07 |  93.7500 | 62.5000 | 62.5000 |  131678 B |
    |  SystemLinq |  ImmutableList |        100000 | 13,408,467.4 ns | 265,016.342 ns | 535,346.082 ns | 13,503,228.9 ns |  1.00 |    0.00 |  93.7500 | 62.5000 | 62.5000 |  131794 B |
    | CisternLinq |  ImmutableList |        100000 | 13,486,344.2 ns | 266,937.103 ns | 520,640.319 ns | 13,514,497.7 ns |  1.01 |    0.05 |  93.7500 | 62.5000 | 62.5000 |  131872 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |            10 |        237.4 ns |       3.937 ns |       3.490 ns |        238.7 ns |  0.67 |    0.01 |   0.0682 |       - |       - |     216 B |
    |  SystemLinq |     FSharpList |            10 |        355.2 ns |       3.636 ns |       3.401 ns |        354.2 ns |  1.00 |    0.00 |   0.1068 |       - |       - |     336 B |
    | CisternLinq |     FSharpList |            10 |        533.9 ns |       5.660 ns |       5.294 ns |        536.0 ns |  1.50 |    0.02 |   0.1068 |       - |       - |     336 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |          1000 |     25,732.2 ns |     476.351 ns |     445.579 ns |     25,803.7 ns |  0.89 |    0.02 |   5.2795 |       - |       - |   16640 B |
    |  SystemLinq |     FSharpList |          1000 |     28,988.9 ns |     332.620 ns |     311.133 ns |     29,008.1 ns |  1.00 |    0.00 |   5.3406 |       - |       - |   16760 B |
    | CisternLinq |     FSharpList |          1000 |     30,415.7 ns |     534.452 ns |     499.926 ns |     30,529.6 ns |  1.05 |    0.02 |   5.3406 |       - |       - |   16762 B |
    |             |                |               |                 |                |                |                 |       |         |          |         |         |           |
    |     ForLoop |     FSharpList |        100000 |  5,255,665.8 ns | 113,427.645 ns | 334,443.970 ns |  5,246,530.9 ns |  0.96 |    0.08 |  93.7500 | 54.6875 | 54.6875 |  131779 B |
    |  SystemLinq |     FSharpList |        100000 |  5,468,941.8 ns | 125,754.996 ns | 370,791.444 ns |  5,478,027.0 ns |  1.00 |    0.00 | 101.5625 | 62.5000 | 62.5000 |  131876 B |
    | CisternLinq |     FSharpList |        100000 |  5,368,803.3 ns | 123,104.967 ns | 362,977.773 ns |  5,387,994.9 ns |  0.99 |    0.10 |  93.7500 | 54.6875 | 54.6875 |  131937 B |
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
