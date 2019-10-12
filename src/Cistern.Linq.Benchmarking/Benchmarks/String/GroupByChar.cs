using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |           Error |          StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------------:|----------------:|----------------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 |        429.0 ns |       2.3727 ns |       2.1033 ns |  0.40 |    0.00 |    0.3133 |        - |        - |     984 B |
    |  SystemLinq |  False |         10 |      1,070.0 ns |       4.6154 ns |       3.8540 ns |  1.00 |    0.00 |    0.5684 |        - |        - |    1784 B |
    | CisternLinq |  False |         10 |        970.1 ns |       3.3394 ns |       2.9602 ns |  0.91 |    0.00 |    0.5417 |        - |        - |    1704 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 |     24,589.3 ns |     106.1831 ns |      99.3237 ns |  0.57 |    0.00 |   10.2539 |        - |        - |   32232 B |
    |  SystemLinq |  False |       1000 |     43,072.5 ns |      94.0623 ns |      78.5463 ns |  1.00 |    0.00 |   15.3809 |        - |        - |   48376 B |
    | CisternLinq |  False |       1000 |     33,900.2 ns |     138.6678 ns |     129.7099 ns |  0.79 |    0.00 |   10.8643 |        - |        - |   34264 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 34,876,870.4 ns | 444,565.3095 ns | 394,095.5673 ns |  0.75 |    0.01 | 1000.0000 | 571.4286 | 285.7143 | 4572434 B |
    |  SystemLinq |  False |     466544 | 46,750,149.7 ns | 704,353.8534 ns | 658,853.0315 ns |  1.00 |    0.00 | 1363.6364 | 909.0909 | 454.5455 | 5515172 B |
    | CisternLinq |  False |     466544 | 23,273,657.0 ns | 120,360.2342 ns | 100,506.2467 ns |  0.50 |    0.01 |  875.0000 | 625.0000 | 343.7500 | 3424746 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |         10 |        279.8 ns |       0.7592 ns |       0.6730 ns |  0.40 |    0.00 |    0.1783 |        - |        - |     560 B |
    |  SystemLinq |   True |         10 |        705.8 ns |       2.0814 ns |       1.7380 ns |  1.00 |    0.00 |    0.3662 |        - |        - |    1152 B |
    | CisternLinq |   True |         10 |        733.0 ns |       3.5654 ns |       3.3351 ns |  1.04 |    0.00 |    0.3948 |        - |        - |    1240 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 |     16,955.8 ns |      78.8902 ns |      69.9341 ns |  0.60 |    0.00 |    8.6365 |        - |        - |   27200 B |
    |  SystemLinq |   True |       1000 |     28,262.9 ns |     155.3567 ns |     145.3208 ns |  1.00 |    0.00 |   11.8103 |        - |        - |   37096 B |
    | CisternLinq |   True |       1000 |     22,658.0 ns |     153.2309 ns |     119.6326 ns |  0.80 |    0.01 |    9.9487 |        - |        - |   31256 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 13,406,471.2 ns | 263,175.7255 ns | 342,202.7810 ns |  0.74 |    0.03 | 1015.6250 | 593.7500 | 281.2500 | 4572622 B |
    |  SystemLinq |   True |     466544 | 17,981,116.6 ns | 337,039.7131 ns | 331,018.0645 ns |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 | 5515224 B |
    | CisternLinq |   True |     466544 | 10,245,295.8 ns | 128,876.1041 ns | 120,550.7877 ns |  0.57 |    0.01 |  640.6250 | 593.7500 | 312.5000 | 2100863 B |
    */
    [CoreJob, MemoryDiagnoser]
    public class String_GroupByChar : StringsBenchmarkBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var answer = new DesiredShape();

            foreach (var n in Words)
            {
                if (!answer.TryGetValue(n[0], out var words))
                {
                    words = new List<string>();
                    answer[n[0]] = words;
                }
                words.Add(n);
            }

            return answer;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return
                System.Linq.Enumerable.ToDictionary(
                    System.Linq.Enumerable.GroupBy(Words, w => w[0]),
                        ws => ws.Key,
                        ws => System.Linq.Enumerable.ToList(ws));
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Words
                .GroupBy(w => w[0])
                .ToDictionary(ws => ws.Key, ws => ws.ToList());
        }
    }
}
