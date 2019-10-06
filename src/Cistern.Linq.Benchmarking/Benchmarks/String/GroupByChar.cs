using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |---------:|----------:|----------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 | 36.15 ms | 0.5270 ms | 0.4929 ms |  0.81 |    0.02 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |         10 | 44.78 ms | 0.8593 ms | 0.8440 ms |  1.00 |    0.00 | 1363.6364 | 818.1818 | 454.5455 |   5.26 MB |
    | CisternLinq |  False |         10 | 37.07 ms | 0.2016 ms | 0.1886 ms |  0.83 |    0.02 |  928.5714 | 642.8571 | 357.1429 |   3.27 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 | 35.35 ms | 0.2072 ms | 0.1837 ms |  0.81 |    0.02 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |       1000 | 43.43 ms | 0.8387 ms | 0.8613 ms |  1.00 |    0.00 | 1333.3333 | 916.6667 | 416.6667 |   5.26 MB |
    | CisternLinq |  False |       1000 | 36.85 ms | 0.1899 ms | 0.1777 ms |  0.85 |    0.02 |  928.5714 | 642.8571 | 357.1429 |   3.27 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 35.36 ms | 0.3852 ms | 0.3603 ms |  0.83 |    0.01 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |     466544 | 42.63 ms | 0.5676 ms | 0.5031 ms |  1.00 |    0.00 | 1333.3333 | 916.6667 | 416.6667 |   5.26 MB |
    | CisternLinq |  False |     466544 | 37.00 ms | 0.2293 ms | 0.2145 ms |  0.87 |    0.01 |  928.5714 | 642.8571 | 357.1429 |   3.27 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |         10 | 13.30 ms | 0.2589 ms | 0.4180 ms |  0.75 |    0.03 | 1015.6250 | 578.1250 | 281.2500 |   4.36 MB |
    |  SystemLinq |   True |         10 | 17.83 ms | 0.3489 ms | 0.5329 ms |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 |   5.26 MB |
    | CisternLinq |   True |         10 | 10.74 ms | 0.1925 ms | 0.1800 ms |  0.61 |    0.02 |  625.0000 | 546.8750 | 296.8750 |      2 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 | 13.75 ms | 0.1690 ms | 0.1581 ms |  0.77 |    0.03 | 1015.6250 | 593.7500 | 281.2500 |   4.36 MB |
    |  SystemLinq |   True |       1000 | 17.82 ms | 0.3517 ms | 0.4930 ms |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 |   5.26 MB |
    | CisternLinq |   True |       1000 | 10.82 ms | 0.2499 ms | 0.2566 ms |  0.61 |    0.02 |  640.6250 | 593.7500 | 312.5000 |      2 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 13.77 ms | 0.2681 ms | 0.3088 ms |  0.76 |    0.04 | 1015.6250 | 593.7500 | 281.2500 |   4.36 MB |
    |  SystemLinq |   True |     466544 | 18.05 ms | 0.3793 ms | 0.6232 ms |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 |   5.26 MB |
    | CisternLinq |   True |     466544 | 10.66 ms | 0.2059 ms | 0.1926 ms |  0.59 |    0.03 |  625.0000 | 546.8750 | 296.8750 |      2 MB |
    */
    [CoreJob, MemoryDiagnoser]
    public class GroupByChar : StringsBenchmarkBase
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
