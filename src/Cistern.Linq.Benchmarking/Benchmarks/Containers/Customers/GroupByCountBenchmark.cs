using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<(string State, int Count)>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |            Mean |          Error |         StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |----------------:|---------------:|---------------:|------:|--------:|---------:|---------:|------:|----------:|
    |     ForLoop |          Array |            10 |        993.1 ns |       8.729 ns |       8.166 ns |  0.52 |    0.01 |   0.2460 |        - |     - |     776 B |
    |  SystemLinq |          Array |            10 |      1,919.3 ns |      13.136 ns |      12.288 ns |  1.00 |    0.00 |   0.4196 |        - |     - |    1328 B |
    | CisternLinq |          Array |            10 |      1,398.9 ns |       9.692 ns |       9.066 ns |  0.73 |    0.01 |   0.4463 |        - |     - |    1408 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |          Array |          1000 |     67,704.2 ns |     495.341 ns |     463.342 ns |  0.95 |    0.01 |   0.4883 |        - |     - |    1584 B |
    |  SystemLinq |          Array |          1000 |     71,239.3 ns |     442.446 ns |     392.217 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27808 B |
    | CisternLinq |          Array |          1000 |     60,834.8 ns |     386.278 ns |     361.325 ns |  0.85 |    0.01 |   8.1787 |        - |     - |   25736 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |          Array |        100000 | 11,116,081.3 ns |  69,164.724 ns |  57,755.677 ns |  0.98 |    0.01 |        - |        - |     - |    1584 B |
    |  SystemLinq |          Array |        100000 | 11,370,354.8 ns | 105,926.386 ns |  93,900.982 ns |  1.00 |    0.00 | 328.1250 | 156.2500 |     - | 1841400 B |
    | CisternLinq |          Array |        100000 |  9,915,691.3 ns |  93,207.424 ns |  87,186.282 ns |  0.87 |    0.01 | 328.1250 | 156.2500 |     - | 1839416 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |           List |            10 |      1,022.6 ns |       8.533 ns |       7.125 ns |  0.51 |    0.01 |   0.2480 |        - |     - |     784 B |
    |  SystemLinq |           List |            10 |      1,987.5 ns |      17.046 ns |      15.945 ns |  1.00 |    0.00 |   0.4234 |        - |     - |    1336 B |
    | CisternLinq |           List |            10 |      1,503.8 ns |      12.985 ns |      12.146 ns |  0.76 |    0.01 |   0.4463 |        - |     - |    1408 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |           List |          1000 |     71,226.4 ns |     466.420 ns |     436.289 ns |  0.90 |    0.01 |   0.4883 |        - |     - |    1592 B |
    |  SystemLinq |           List |          1000 |     79,130.9 ns |     590.623 ns |     552.469 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27816 B |
    | CisternLinq |           List |          1000 |     70,295.6 ns |     524.308 ns |     490.438 ns |  0.89 |    0.01 |   8.1787 |        - |     - |   25736 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |           List |        100000 | 11,038,132.9 ns | 136,804.663 ns | 127,967.167 ns |  0.93 |    0.01 |        - |        - |     - |    1592 B |
    |  SystemLinq |           List |        100000 | 11,841,735.2 ns |  59,105.240 ns |  52,395.255 ns |  1.00 |    0.00 | 312.5000 | 156.2500 |     - | 1841408 B |
    | CisternLinq |           List |        100000 | 11,099,900.3 ns |  60,638.996 ns |  56,721.754 ns |  0.94 |    0.01 | 328.1250 | 156.2500 |     - | 1839416 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     Enumerable |            10 |      1,007.6 ns |       8.548 ns |       7.996 ns |  0.50 |    0.00 |   0.2556 |        - |     - |     808 B |
    |  SystemLinq |     Enumerable |            10 |      2,002.8 ns |      10.733 ns |      10.040 ns |  1.00 |    0.00 |   0.4311 |        - |     - |    1360 B |
    | CisternLinq |     Enumerable |            10 |      1,658.9 ns |      11.803 ns |      11.041 ns |  0.83 |    0.01 |   0.4673 |        - |     - |    1472 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     Enumerable |          1000 |     71,161.6 ns |     681.023 ns |     637.029 ns |  0.92 |    0.01 |   0.4883 |        - |     - |    1616 B |
    |  SystemLinq |     Enumerable |          1000 |     77,633.4 ns |     508.966 ns |     476.087 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27840 B |
    | CisternLinq |     Enumerable |          1000 |     71,388.0 ns |     370.372 ns |     346.446 ns |  0.92 |    0.01 |   8.1787 |        - |     - |   25804 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     Enumerable |        100000 | 11,307,512.1 ns | 143,596.506 ns | 134,320.261 ns |  1.00 |    0.02 |        - |        - |     - |    1616 B |
    |  SystemLinq |     Enumerable |        100000 | 11,254,670.9 ns | 119,192.390 ns | 111,492.635 ns |  1.00 |    0.00 | 312.5000 | 156.2500 |     - | 1841432 B |
    | CisternLinq |     Enumerable |        100000 | 11,031,442.1 ns |  63,178.848 ns |  59,097.533 ns |  0.98 |    0.01 | 328.1250 | 156.2500 |     - | 1839653 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop | ImmutableArray |            10 |        978.0 ns |       8.389 ns |       7.847 ns |  0.52 |    0.01 |   0.2460 |        - |     - |     776 B |
    |  SystemLinq | ImmutableArray |            10 |      1,874.7 ns |      18.783 ns |      17.569 ns |  1.00 |    0.00 |   0.4215 |        - |     - |    1328 B |
    | CisternLinq | ImmutableArray |            10 |      1,503.3 ns |      11.535 ns |      10.790 ns |  0.80 |    0.01 |   0.4463 |        - |     - |    1408 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop | ImmutableArray |          1000 |     67,718.8 ns |     581.746 ns |     544.166 ns |  0.91 |    0.01 |   0.4883 |        - |     - |    1584 B |
    |  SystemLinq | ImmutableArray |          1000 |     74,256.6 ns |     478.457 ns |     447.549 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27808 B |
    | CisternLinq | ImmutableArray |          1000 |     63,502.7 ns |     399.644 ns |     373.827 ns |  0.86 |    0.01 |   8.1787 |        - |     - |   25739 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop | ImmutableArray |        100000 | 10,772,924.3 ns |  92,573.768 ns |  86,593.560 ns |  0.91 |    0.01 |        - |        - |     - |    1584 B |
    |  SystemLinq | ImmutableArray |        100000 | 11,870,239.7 ns |  96,137.352 ns |  89,926.939 ns |  1.00 |    0.00 | 312.5000 | 156.2500 |     - | 1841400 B |
    | CisternLinq | ImmutableArray |        100000 | 10,435,433.0 ns | 103,278.616 ns |  96,606.882 ns |  0.88 |    0.01 | 328.1250 | 156.2500 |     - | 1839520 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |  ImmutableList |            10 |      2,039.5 ns |      15.311 ns |      14.322 ns |  0.67 |    0.01 |   0.2556 |        - |     - |     816 B |
    |  SystemLinq |  ImmutableList |            10 |      3,062.1 ns |      20.299 ns |      17.995 ns |  1.00 |    0.00 |   0.4311 |        - |     - |    1368 B |
    | CisternLinq |  ImmutableList |            10 |      2,787.7 ns |      14.531 ns |      13.592 ns |  0.91 |    0.01 |   0.4463 |        - |     - |    1408 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |  ImmutableList |          1000 |    157,491.9 ns |     951.148 ns |     889.704 ns |  0.96 |    0.01 |   0.4883 |        - |     - |    1624 B |
    |  SystemLinq |  ImmutableList |          1000 |    163,634.7 ns |   1,092.502 ns |   1,021.927 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27848 B |
    | CisternLinq |  ImmutableList |          1000 |    156,676.4 ns |   1,542.205 ns |   1,442.579 ns |  0.96 |    0.01 |   8.0566 |        - |     - |   25738 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |  ImmutableList |        100000 | 20,420,006.9 ns | 233,895.850 ns | 218,786.323 ns |  1.04 |    0.02 |        - |        - |     - |    1624 B |
    |  SystemLinq |  ImmutableList |        100000 | 19,601,181.5 ns | 195,946.687 ns | 163,624.358 ns |  1.00 |    0.00 | 312.5000 | 156.2500 |     - | 1841440 B |
    | CisternLinq |  ImmutableList |        100000 | 19,341,542.5 ns | 219,591.037 ns | 205,405.593 ns |  0.99 |    0.02 | 312.5000 | 156.2500 |     - | 1839518 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     FSharpList |            10 |        994.1 ns |       9.082 ns |       8.496 ns |  0.50 |    0.01 |   0.2480 |        - |     - |     784 B |
    |  SystemLinq |     FSharpList |            10 |      1,969.6 ns |      17.517 ns |      16.385 ns |  1.00 |    0.00 |   0.4234 |        - |     - |    1336 B |
    | CisternLinq |     FSharpList |            10 |      1,596.2 ns |      12.671 ns |      11.853 ns |  0.81 |    0.01 |   0.4463 |        - |     - |    1408 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     FSharpList |          1000 |     70,685.2 ns |     770.220 ns |     720.464 ns |  0.96 |    0.01 |   0.4883 |        - |     - |    1592 B |
    |  SystemLinq |     FSharpList |          1000 |     73,368.7 ns |     619.958 ns |     579.909 ns |  1.00 |    0.00 |   8.7891 |        - |     - |   27816 B |
    | CisternLinq |     FSharpList |          1000 |     69,771.0 ns |     449.921 ns |     420.856 ns |  0.95 |    0.01 |   8.1787 |        - |     - |   25738 B |
    |             |                |               |                 |                |                |       |         |          |          |       |           |
    |     ForLoop |     FSharpList |        100000 | 10,353,447.7 ns | 195,342.668 ns | 191,852.620 ns |  1.03 |    0.03 |        - |        - |     - |    1592 B |
    |  SystemLinq |     FSharpList |        100000 | 10,056,870.0 ns | 203,804.095 ns | 298,733.399 ns |  1.00 |    0.00 | 312.5000 | 156.2500 |     - | 1841408 B |
    | CisternLinq |     FSharpList |        100000 |  9,498,953.2 ns | 222,682.183 ns | 228,678.207 ns |  0.94 |    0.03 | 328.1250 | 156.2500 |     - | 1839518 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_GroupByCountBenchmark : CustomersBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var counts = new Dictionary<string, int>();
            foreach (var c in Customers)
            {
                counts.TryGetValue(c.State, out var count);
                counts[c.State] = count + 1;
            }

            var results = new DesiredShape();
            foreach (var c in counts)
                results.Add((c.Key, c.Value));
            return results;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.Select(
                        System.Linq.Enumerable.GroupBy(
                            Customers,
                            c => c.State),
                        c => (c.Key, c.Count()))
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Customers
                .GroupBy(c => c.State)
                .Select(c => (c.Key, c.Count()))
                .ToList();
        }

    }
}
