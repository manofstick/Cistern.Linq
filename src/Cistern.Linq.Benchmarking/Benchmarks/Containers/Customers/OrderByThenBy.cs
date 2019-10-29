using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<Cistern.Linq.Benchmarking.Data.DummyData.Customer>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |         Mean |          Error |         StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |-------------:|---------------:|---------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |          Array |             0 |     138.9 ns |      2.0681 ns |      1.9345 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |          Array |             0 |     183.0 ns |      1.6689 ns |      1.4794 ns |  1.32 |    0.02 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |             1 |     272.8 ns |      3.1886 ns |      2.9826 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |          Array |             1 |     575.3 ns |      4.7032 ns |      4.3994 ns |  2.11 |    0.03 |  0.1678 |     - |     - |     528 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |            10 |   1,264.2 ns |     13.6809 ns |     12.7971 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |          Array |            10 |   1,480.9 ns |     22.0863 ns |     20.6595 ns |  1.17 |    0.02 |  0.2899 |     - |     - |     912 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |           100 |  47,350.0 ns |    442.1835 ns |    413.6187 ns |  1.00 |    0.00 |  1.2207 |     - |     - |    4128 B |
    | CisternLinq |          Array |           100 |  37,932.8 ns |    529.7747 ns |    495.5516 ns |  0.80 |    0.01 |  1.2817 |     - |     - |    4152 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |          Array |          1000 | 877,996.4 ns |  9,150.3502 ns |  8,559.2432 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |          Array |          1000 | 715,789.4 ns |  7,388.9122 ns |  6,911.5930 ns |  0.82 |    0.01 | 10.7422 |     - |     - |   36488 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |             0 |     112.4 ns |      0.8396 ns |      0.7853 ns |  1.00 |    0.00 |  0.0459 |     - |     - |     144 B |
    | CisternLinq |           List |             0 |     166.3 ns |      3.4598 ns |      3.3980 ns |  1.48 |    0.03 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |             1 |     234.6 ns |      0.9544 ns |      0.8460 ns |  1.00 |    0.00 |  0.1605 |     - |     - |     504 B |
    | CisternLinq |           List |             1 |     561.0 ns |      7.3630 ns |      6.8874 ns |  2.39 |    0.03 |  0.1678 |     - |     - |     528 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |            10 |   1,227.1 ns |     14.0462 ns |     12.4516 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |           List |            10 |   1,444.4 ns |     18.8434 ns |     17.6262 ns |  1.18 |    0.02 |  0.2899 |     - |     - |     912 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |           100 |  47,914.7 ns |    499.8603 ns |    467.5696 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |           List |           100 |  37,750.3 ns |    440.9976 ns |    412.5094 ns |  0.79 |    0.01 |  1.2817 |     - |     - |    4152 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |           List |          1000 | 871,324.8 ns | 11,810.4004 ns | 10,469.6124 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |           List |          1000 | 708,925.0 ns |  9,465.4231 ns |  8,853.9627 ns |  0.82 |    0.01 | 10.7422 |     - |     - |   36488 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             0 |     131.8 ns |      2.6635 ns |      2.7352 ns |  1.00 |    0.00 |  0.0663 |     - |     - |     208 B |
    | CisternLinq |     Enumerable |             0 |     210.7 ns |      4.2087 ns |      3.9368 ns |  1.60 |    0.03 |  0.0737 |     - |     - |     232 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             1 |     252.6 ns |      3.0261 ns |      2.8306 ns |  1.00 |    0.00 |  0.1884 |     - |     - |     592 B |
    | CisternLinq |     Enumerable |             1 |     601.9 ns |      7.5014 ns |      7.0168 ns |  2.38 |    0.04 |  0.1879 |     - |     - |     592 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |            10 |   1,394.5 ns |     11.5079 ns |     10.7645 ns |  1.00 |    0.00 |  0.3643 |     - |     - |    1144 B |
    | CisternLinq |     Enumerable |            10 |   1,634.5 ns |     17.9682 ns |     16.8074 ns |  1.17 |    0.01 |  0.3090 |     - |     - |     976 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |           100 |  52,555.2 ns |    236.9909 ns |    210.0863 ns |  1.00 |    0.00 |  1.7090 |     - |     - |    5528 B |
    | CisternLinq |     Enumerable |           100 |  40,383.9 ns |    261.6114 ns |    231.9117 ns |  0.77 |    0.00 |  1.5869 |     - |     - |    5072 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |          1000 | 879,250.2 ns |  6,369.9530 ns |  5,958.4580 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45136 B |
    | CisternLinq |     Enumerable |          1000 | 711,811.2 ns |  5,780.8880 ns |  5,124.6066 ns |  0.81 |    0.01 | 14.6484 |     - |     - |   47440 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             0 |     121.8 ns |      2.5253 ns |      2.3622 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq | ImmutableArray |             0 |     172.8 ns |      2.6818 ns |      2.5085 ns |  1.42 |    0.03 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             1 |     239.3 ns |      3.8601 ns |      3.6107 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq | ImmutableArray |             1 |     572.2 ns |      6.4898 ns |      6.0706 ns |  2.39 |    0.04 |  0.1678 |     - |     - |     528 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |            10 |   1,192.0 ns |     14.4875 ns |     13.5516 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq | ImmutableArray |            10 |   1,492.8 ns |     18.7697 ns |     17.5572 ns |  1.25 |    0.02 |  0.2899 |     - |     - |     912 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |           100 |  50,775.9 ns |    774.3348 ns |    724.3133 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq | ImmutableArray |           100 |  38,358.5 ns |    437.3704 ns |    409.1166 ns |  0.76 |    0.01 |  1.2817 |     - |     - |    4152 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |          1000 | 825,042.5 ns |  9,576.6790 ns |  8,958.0314 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq | ImmutableArray |          1000 | 707,557.8 ns |  8,367.3879 ns |  7,826.8598 ns |  0.86 |    0.01 | 10.7422 |     - |     - |   36488 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             0 |     121.5 ns |      2.2708 ns |      2.1242 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |  ImmutableList |             0 |     181.6 ns |      1.9804 ns |      1.8525 ns |  1.50 |    0.03 |  0.0532 |     - |     - |     168 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             1 |     578.2 ns |      7.6749 ns |      7.1791 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |  ImmutableList |             1 |     874.0 ns |     10.2516 ns |      9.5894 ns |  1.51 |    0.02 |  0.1669 |     - |     - |     528 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |            10 |   2,321.5 ns |     29.6324 ns |     27.7181 ns |  1.00 |    0.00 |  0.2785 |     - |     - |     888 B |
    | CisternLinq |  ImmutableList |            10 |   2,577.0 ns |     33.1017 ns |     30.9633 ns |  1.11 |    0.02 |  0.2899 |     - |     - |     912 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |           100 |  58,585.6 ns |    729.0081 ns |    681.9147 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |  ImmutableList |           100 |  45,798.0 ns |    750.8636 ns |    702.3583 ns |  0.78 |    0.01 |  1.2817 |     - |     - |    4152 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |          1000 | 960,933.2 ns | 13,297.3682 ns | 12,438.3664 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |  ImmutableList |          1000 | 767,689.0 ns |  9,719.4883 ns |  9,091.6153 ns |  0.80 |    0.01 | 10.7422 |     - |     - |   36488 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             0 |     131.0 ns |      1.8942 ns |      1.7718 ns |  1.00 |    0.00 |  0.0584 |     - |     - |     184 B |
    | CisternLinq |     FSharpList |             0 |     212.7 ns |      2.4218 ns |      2.2653 ns |  1.62 |    0.03 |  0.0660 |     - |     - |     208 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             1 |     245.4 ns |      3.1924 ns |      2.9862 ns |  1.00 |    0.00 |  0.1807 |     - |     - |     568 B |
    | CisternLinq |     FSharpList |             1 |     619.5 ns |      8.5479 ns |      7.9957 ns |  2.52 |    0.05 |  0.1802 |     - |     - |     568 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |            10 |   1,386.2 ns |     16.2262 ns |     15.1780 ns |  1.00 |    0.00 |  0.3567 |     - |     - |    1120 B |
    | CisternLinq |     FSharpList |            10 |   1,605.7 ns |     19.5979 ns |     18.3319 ns |  1.16 |    0.01 |  0.3033 |     - |     - |     952 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |           100 |  48,595.3 ns |    580.7633 ns |    543.2463 ns |  1.00 |    0.00 |  1.7090 |     - |     - |    5504 B |
    | CisternLinq |     FSharpList |           100 |  38,623.6 ns |    465.9093 ns |    435.8118 ns |  0.79 |    0.01 |  1.5869 |     - |     - |    5048 B |
    |             |                |               |              |                |                |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |          1000 | 902,457.6 ns | 15,210.4812 ns | 12,701.4407 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45112 B |
    | CisternLinq |     FSharpList |          1000 | 729,849.2 ns |  8,199.4232 ns |  7,669.7456 ns |  0.81 |    0.02 | 14.6484 |     - |     - |   47416 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class Containers_OrderByThenBy : CustomersBase
    {
        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return 
                System.Linq.Enumerable.ToList(
                    System.Linq.Enumerable.ThenBy(
                        System.Linq.Enumerable.OrderBy(
                            Customers,
                            c => c.State),
                        c => c.Name)
                    );
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Customers
                .OrderBy(c => c.State)
                .ThenBy(c => c.Name)
                .ToList();
        }

    }
}
