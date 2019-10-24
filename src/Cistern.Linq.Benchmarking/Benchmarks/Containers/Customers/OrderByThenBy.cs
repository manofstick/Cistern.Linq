using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.List<Cistern.Linq.Benchmarking.Data.DummyData.Customer>;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Customers
{
    /*
    |      Method |  ContainerType | CustomerCount |           Mean |         Error |        StdDev | Ratio | RatioSD |   Gen 0 | Gen 1 | Gen 2 | Allocated |
    |------------ |--------------- |-------------- |---------------:|--------------:|--------------:|------:|--------:|--------:|------:|------:|----------:|
    |  SystemLinq |          Array |             0 |       136.2 ns |      1.290 ns |      1.207 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |          Array |             0 |       250.1 ns |      3.702 ns |      3.282 ns |  1.84 |    0.03 |  0.0658 |     - |     - |     208 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |             1 |       253.8 ns |      3.953 ns |      3.697 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |          Array |             1 |       277.5 ns |      3.965 ns |      3.709 ns |  1.09 |    0.02 |  0.0939 |     - |     - |     296 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |            10 |     1,258.3 ns |     15.198 ns |     14.216 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |          Array |            10 |     1,419.4 ns |     16.139 ns |     15.097 ns |  1.13 |    0.02 |  0.2060 |     - |     - |     656 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |           100 |    51,195.6 ns |    501.603 ns |    469.200 ns |  1.00 |    0.00 |  1.2207 |     - |     - |    4128 B |
    | CisternLinq |          Array |           100 |    59,197.3 ns |    687.412 ns |    643.006 ns |  1.16 |    0.02 |  1.3428 |     - |     - |    4256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |          Array |          1000 |   819,168.2 ns | 13,743.875 ns | 12,856.029 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |          Array |          1000 |   951,299.5 ns | 13,780.694 ns | 12,890.470 ns |  1.16 |    0.03 | 12.6953 |     - |     - |   40256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |             0 |       111.2 ns |      2.199 ns |      2.444 ns |  1.00 |    0.00 |  0.0458 |     - |     - |     144 B |
    | CisternLinq |           List |             0 |       223.8 ns |      3.845 ns |      3.597 ns |  2.02 |    0.06 |  0.0660 |     - |     - |     208 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |             1 |       234.6 ns |      3.320 ns |      3.106 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |           List |             1 |       258.2 ns |      3.884 ns |      3.633 ns |  1.10 |    0.02 |  0.0939 |     - |     - |     296 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |            10 |     1,240.2 ns |     19.207 ns |     17.027 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |           List |            10 |     1,394.8 ns |     14.378 ns |     13.449 ns |  1.13 |    0.02 |  0.2079 |     - |     - |     656 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |           100 |    49,394.2 ns |    533.079 ns |    498.642 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |           List |           100 |    53,855.2 ns |    664.352 ns |    621.435 ns |  1.09 |    0.02 |  1.3428 |     - |     - |    4256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |           List |          1000 |   872,110.6 ns |  4,126.229 ns |  3,859.677 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |           List |          1000 | 1,001,595.1 ns | 16,940.555 ns | 15,846.206 ns |  1.15 |    0.02 | 11.7188 |     - |     - |   40256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             0 |       132.9 ns |      1.172 ns |      1.097 ns |  1.00 |    0.00 |  0.0663 |     - |     - |     208 B |
    | CisternLinq |     Enumerable |             0 |       368.9 ns |      5.048 ns |      4.721 ns |  2.78 |    0.05 |  0.1016 |     - |     - |     320 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |             1 |       256.0 ns |      3.344 ns |      3.128 ns |  1.00 |    0.00 |  0.1884 |     - |     - |     592 B |
    | CisternLinq |     Enumerable |             1 |       431.6 ns |      6.095 ns |      5.702 ns |  1.69 |    0.03 |  0.1478 |     - |     - |     464 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |            10 |     1,461.5 ns |     12.016 ns |     11.239 ns |  1.00 |    0.00 |  0.3624 |     - |     - |    1144 B |
    | CisternLinq |     Enumerable |            10 |     1,844.9 ns |     27.237 ns |     25.477 ns |  1.26 |    0.02 |  0.3185 |     - |     - |    1000 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |           100 |    49,381.8 ns |    735.381 ns |    614.076 ns |  1.00 |    0.00 |  1.6479 |     - |     - |    5528 B |
    | CisternLinq |     Enumerable |           100 |    56,392.8 ns |    744.376 ns |    696.290 ns |  1.14 |    0.02 |  1.7700 |     - |     - |    5568 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     Enumerable |          1000 |   861,537.5 ns |  9,176.638 ns |  8,583.833 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45136 B |
    | CisternLinq |     Enumerable |          1000 |   999,532.8 ns | 12,517.752 ns | 11,709.113 ns |  1.16 |    0.01 | 13.6719 |     - |     - |   48896 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             0 |       118.2 ns |      1.755 ns |      1.642 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq | ImmutableArray |             0 |       242.9 ns |      4.305 ns |      4.027 ns |  2.06 |    0.05 |  0.0658 |     - |     - |     208 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |             1 |       238.8 ns |      3.461 ns |      3.238 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq | ImmutableArray |             1 |       269.8 ns |      2.876 ns |      2.690 ns |  1.13 |    0.02 |  0.0939 |     - |     - |     296 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |            10 |     1,196.7 ns |     14.652 ns |     13.705 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq | ImmutableArray |            10 |     1,497.0 ns |     15.829 ns |     14.806 ns |  1.25 |    0.02 |  0.2060 |     - |     - |     656 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |           100 |    47,563.2 ns |    435.634 ns |    407.492 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq | ImmutableArray |           100 |    57,340.0 ns |    649.951 ns |    607.965 ns |  1.21 |    0.02 |  1.3428 |     - |     - |    4256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq | ImmutableArray |          1000 |   831,891.5 ns | 11,674.595 ns | 10,920.424 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq | ImmutableArray |          1000 | 1,007,509.5 ns | 14,671.490 ns | 13,723.720 ns |  1.21 |    0.02 | 11.7188 |     - |     - |   40256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             0 |       128.9 ns |      1.265 ns |      1.121 ns |  1.00 |    0.00 |  0.0455 |     - |     - |     144 B |
    | CisternLinq |  ImmutableList |             0 |       242.0 ns |      3.807 ns |      3.561 ns |  1.88 |    0.03 |  0.0663 |     - |     - |     208 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |             1 |       569.2 ns |      8.791 ns |      8.223 ns |  1.00 |    0.00 |  0.1602 |     - |     - |     504 B |
    | CisternLinq |  ImmutableList |             1 |       573.2 ns |      5.808 ns |      5.432 ns |  1.01 |    0.02 |  0.0935 |     - |     - |     296 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |            10 |     2,257.9 ns |     31.535 ns |     29.498 ns |  1.00 |    0.00 |  0.2823 |     - |     - |     888 B |
    | CisternLinq |  ImmutableList |            10 |     2,520.0 ns |     31.848 ns |     29.791 ns |  1.12 |    0.02 |  0.2060 |     - |     - |     656 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |           100 |    54,663.5 ns |    433.268 ns |    384.081 ns |  1.00 |    0.00 |  1.2817 |     - |     - |    4128 B |
    | CisternLinq |  ImmutableList |           100 |    61,824.3 ns |    947.243 ns |    886.052 ns |  1.13 |    0.02 |  1.3428 |     - |     - |    4256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |  ImmutableList |          1000 |   946,660.0 ns |  8,736.656 ns |  7,295.504 ns |  1.00 |    0.00 | 10.7422 |     - |     - |   36528 B |
    | CisternLinq |  ImmutableList |          1000 | 1,039,206.7 ns | 12,892.203 ns | 12,059.375 ns |  1.10 |    0.01 | 11.7188 |     - |     - |   40256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             0 |       126.8 ns |      1.598 ns |      1.494 ns |  1.00 |    0.00 |  0.0587 |     - |     - |     184 B |
    | CisternLinq |     FSharpList |             0 |       390.6 ns |      4.624 ns |      4.325 ns |  3.08 |    0.04 |  0.0811 |     - |     - |     256 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |             1 |       254.7 ns |      3.370 ns |      3.152 ns |  1.00 |    0.00 |  0.1807 |     - |     - |     568 B |
    | CisternLinq |     FSharpList |             1 |       451.3 ns |      6.072 ns |      5.679 ns |  1.77 |    0.03 |  0.1273 |     - |     - |     400 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |            10 |     1,400.4 ns |     18.549 ns |     17.351 ns |  1.00 |    0.00 |  0.3567 |     - |     - |    1120 B |
    | CisternLinq |     FSharpList |            10 |     1,973.8 ns |     33.094 ns |     30.957 ns |  1.41 |    0.03 |  0.2975 |     - |     - |     936 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |           100 |    47,805.8 ns |    631.114 ns |    590.345 ns |  1.00 |    0.00 |  1.6479 |     - |     - |    5504 B |
    | CisternLinq |     FSharpList |           100 |    55,731.4 ns |    830.317 ns |    776.679 ns |  1.17 |    0.02 |  1.7090 |     - |     - |    5504 B |
    |             |                |               |                |               |               |       |         |         |       |       |           |
    |  SystemLinq |     FSharpList |          1000 |   881,736.5 ns | 10,465.884 ns |  9,789.795 ns |  1.00 |    0.00 | 13.6719 |     - |     - |   45112 B |
    | CisternLinq |     FSharpList |          1000 |   973,377.1 ns | 13,129.225 ns | 12,281.085 ns |  1.10 |    0.02 | 14.6484 |     - |     - |   48836 B |
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
