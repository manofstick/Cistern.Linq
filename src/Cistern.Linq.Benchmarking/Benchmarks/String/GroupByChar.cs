using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |     Mean |     Error |    StdDev | Ratio |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |---------:|----------:|----------:|------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 | 37.68 ms | 0.2825 ms | 0.2643 ms |  0.86 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |         10 | 43.68 ms | 0.5382 ms | 0.5034 ms |  1.00 | 1333.3333 | 916.6667 | 416.6667 |   5.26 MB |
    | CisternLinq |  False |         10 | 24.13 ms | 0.2166 ms | 0.1921 ms |  0.55 |  875.0000 | 625.0000 | 343.7500 |   3.27 MB |
    |             |        |            |          |           |           |       |           |          |          |           |
    |     ForLoop |  False |       1000 | 37.63 ms | 0.4461 ms | 0.4173 ms |  0.80 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |       1000 | 47.15 ms | 0.4907 ms | 0.4590 ms |  1.00 | 1363.6364 | 818.1818 | 454.5455 |   5.26 MB |
    | CisternLinq |  False |       1000 | 24.12 ms | 0.1731 ms | 0.1619 ms |  0.51 |  906.2500 | 656.2500 | 375.0000 |   3.27 MB |
    |             |        |            |          |           |           |       |           |          |          |           |
    |     ForLoop |  False |     466544 | 37.72 ms | 0.3423 ms | 0.3201 ms |  0.86 | 1000.0000 | 571.4286 | 285.7143 |   4.36 MB |
    |  SystemLinq |  False |     466544 | 43.62 ms | 0.6605 ms | 0.6179 ms |  1.00 | 1333.3333 | 916.6667 | 416.6667 |   5.26 MB |
    | CisternLinq |  False |     466544 | 24.08 ms | 0.1342 ms | 0.1190 ms |  0.55 |  875.0000 | 625.0000 | 343.7500 |   3.27 MB |
    |             |        |            |          |           |           |       |           |          |          |           |
    |     ForLoop |   True |         10 | 10.46 ms | 0.1551 ms | 0.1375 ms |  0.64 | 1000.0000 | 578.1250 | 265.6250 |   4.36 MB |
    |  SystemLinq |   True |         10 | 16.42 ms | 0.1736 ms | 0.1624 ms |  1.00 | 1281.2500 | 812.5000 | 406.2500 |   5.26 MB |
    | CisternLinq |   True |         10 | 10.72 ms | 0.2098 ms | 0.2416 ms |  0.66 |  656.2500 | 640.6250 | 328.1250 |      2 MB |
    |             |        |            |          |           |           |       |           |          |          |           |
    |     ForLoop |   True |       1000 | 10.52 ms | 0.1774 ms | 0.1659 ms |  0.64 | 1000.0000 | 578.1250 | 265.6250 |   4.36 MB |
    |  SystemLinq |   True |       1000 | 16.36 ms | 0.1500 ms | 0.1404 ms |  1.00 | 1281.2500 | 812.5000 | 406.2500 |   5.26 MB |
    | CisternLinq |   True |       1000 | 10.81 ms | 0.2125 ms | 0.2087 ms |  0.66 |  625.0000 | 515.6250 | 296.8750 |      2 MB |
    |             |        |            |          |           |           |       |           |          |          |           |
    |     ForLoop |   True |     466544 | 10.48 ms | 0.1278 ms | 0.1133 ms |  0.64 | 1000.0000 | 562.5000 | 265.6250 |   4.36 MB |
    |  SystemLinq |   True |     466544 | 16.47 ms | 0.1587 ms | 0.1485 ms |  1.00 | 1250.0000 | 781.2500 | 375.0000 |   5.26 MB |
    | CisternLinq |   True |     466544 | 10.87 ms | 0.1692 ms | 0.1583 ms |  0.66 |  609.3750 | 531.2500 | 281.2500 |      2 MB |
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
