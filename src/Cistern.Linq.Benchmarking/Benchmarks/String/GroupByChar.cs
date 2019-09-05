using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |---------:|----------:|----------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 | 41.11 ms | 0.3338 ms | 0.3122 ms |  0.92 |    0.01 |  923.0769 | 538.4615 | 230.7692 |  10.24 MB |
    |  SystemLinq |  False |         10 | 44.67 ms | 0.3244 ms | 0.2875 ms |  1.00 |    0.00 | 1250.0000 | 750.0000 | 333.3333 |  13.81 MB |
    | CisternLinq |  False |         10 | 40.77 ms | 0.3671 ms | 0.3434 ms |  0.91 |    0.01 |  923.0769 | 615.3846 | 384.6154 |  10.82 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 | 40.91 ms | 0.3328 ms | 0.3113 ms |  0.90 |    0.01 |  923.0769 | 538.4615 | 230.7692 |  10.24 MB |
    |  SystemLinq |  False |       1000 | 45.53 ms | 0.6786 ms | 0.6348 ms |  1.00 |    0.00 | 1272.7273 | 818.1818 | 363.6364 |  13.81 MB |
    | CisternLinq |  False |       1000 | 40.70 ms | 0.3302 ms | 0.3089 ms |  0.89 |    0.01 |  923.0769 | 615.3846 | 384.6154 |  10.82 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 40.86 ms | 0.2384 ms | 0.2230 ms |  0.91 |    0.01 |  923.0769 | 538.4615 | 230.7692 |  10.24 MB |
    |  SystemLinq |  False |     466544 | 44.97 ms | 0.3479 ms | 0.3254 ms |  1.00 |    0.00 | 1333.3333 | 833.3333 | 416.6667 |  13.81 MB |
    | CisternLinq |  False |     466544 | 41.23 ms | 0.3808 ms | 0.3562 ms |  0.92 |    0.01 |  916.6667 | 583.3333 | 333.3333 |  10.82 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |         10 | 11.49 ms | 0.1995 ms | 0.1866 ms |  0.64 |    0.01 | 1015.6250 | 593.7500 | 281.2500 |  10.24 MB |
    |  SystemLinq |   True |         10 | 18.09 ms | 0.1949 ms | 0.1728 ms |  1.00 |    0.00 | 1281.2500 | 843.7500 | 406.2500 |  13.81 MB |
    | CisternLinq |   True |         10 | 13.39 ms | 0.1534 ms | 0.1434 ms |  0.74 |    0.01 |  656.2500 | 640.6250 | 328.1250 |    8.8 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 | 11.46 ms | 0.2259 ms | 0.2689 ms |  0.64 |    0.01 | 1015.6250 | 593.7500 | 281.2500 |  10.24 MB |
    |  SystemLinq |   True |       1000 | 18.04 ms | 0.1988 ms | 0.1860 ms |  1.00 |    0.00 | 1281.2500 | 843.7500 | 406.2500 |  13.81 MB |
    | CisternLinq |   True |       1000 | 13.58 ms | 0.2631 ms | 0.2815 ms |  0.75 |    0.02 |  656.2500 | 640.6250 | 328.1250 |    8.8 MB |
    |             |        |            |          |           |           |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 11.43 ms | 0.2264 ms | 0.2607 ms |  0.63 |    0.01 | 1015.6250 | 578.1250 | 281.2500 |  10.24 MB |
    |  SystemLinq |   True |     466544 | 18.17 ms | 0.2621 ms | 0.2452 ms |  1.00 |    0.00 | 1281.2500 | 843.7500 | 406.2500 |  13.81 MB |
    | CisternLinq |   True |     466544 | 13.47 ms | 0.1709 ms | 0.1599 ms |  0.74 |    0.01 |  656.2500 | 640.6250 | 328.1250 |    8.8 MB |
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
