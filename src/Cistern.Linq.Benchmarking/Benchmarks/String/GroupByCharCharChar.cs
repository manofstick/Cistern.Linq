using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<(char, char, char), System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|----------:|---------:|----------:|
    |     ForLoop |  False |         10 |  85.17 ms | 0.7276 ms | 0.6806 ms |  0.85 |    0.01 | 1833.3333 |  833.3333 |        - |  11.92 MB |
    |  SystemLinq |  False |         10 | 100.42 ms | 0.5752 ms | 0.5380 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.07 MB |
    | CisternLinq |  False |         10 |  91.19 ms | 0.4584 ms | 0.4288 ms |  0.91 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |       1000 |  84.80 ms | 0.4681 ms | 0.4150 ms |  0.85 |    0.01 | 1833.3333 |  833.3333 |        - |  11.92 MB |
    |  SystemLinq |  False |       1000 |  99.56 ms | 0.4280 ms | 0.4004 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.07 MB |
    | CisternLinq |  False |       1000 |  91.26 ms | 0.7799 ms | 0.6914 ms |  0.92 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |  False |     466544 |  84.40 ms | 0.5105 ms | 0.4775 ms |  0.85 |    0.01 | 1833.3333 |  833.3333 |        - |  11.92 MB |
    |  SystemLinq |  False |     466544 |  99.70 ms | 0.6233 ms | 0.5831 ms |  1.00 |    0.00 | 3200.0000 | 1600.0000 | 400.0000 |  17.07 MB |
    | CisternLinq |  False |     466544 |  90.76 ms | 0.6273 ms | 0.5868 ms |  0.91 |    0.01 | 2333.3333 | 1166.6667 | 333.3333 |  12.44 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |         10 |  36.56 ms | 0.3519 ms | 0.3292 ms |  0.65 |    0.01 | 2071.4286 |  857.1429 | 214.2857 |  11.92 MB |
    |  SystemLinq |   True |         10 |  56.15 ms | 0.9985 ms | 0.9340 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  17.06 MB |
    | CisternLinq |   True |         10 |  43.29 ms | 0.3095 ms | 0.2744 ms |  0.77 |    0.02 | 1916.6667 |  750.0000 | 166.6667 |  11.55 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |       1000 |  36.83 ms | 0.4364 ms | 0.4082 ms |  0.66 |    0.01 | 2071.4286 |  857.1429 | 214.2857 |  11.92 MB |
    |  SystemLinq |   True |       1000 |  56.07 ms | 0.7530 ms | 0.7044 ms |  1.00 |    0.00 | 3111.1111 | 1333.3333 | 444.4444 |  17.06 MB |
    | CisternLinq |   True |       1000 |  43.49 ms | 0.5065 ms | 0.4737 ms |  0.78 |    0.01 | 1916.6667 |  750.0000 | 166.6667 |  11.55 MB |
    |             |        |            |           |           |           |       |         |           |           |          |           |
    |     ForLoop |   True |     466544 |  36.99 ms | 0.4637 ms | 0.4337 ms |  0.66 |    0.01 | 2071.4286 |  857.1429 | 214.2857 |  11.92 MB |
    |  SystemLinq |   True |     466544 |  55.66 ms | 0.5664 ms | 0.5021 ms |  1.00 |    0.00 | 3100.0000 | 1300.0000 | 400.0000 |  17.06 MB |
    | CisternLinq |   True |     466544 |  43.38 ms | 0.6240 ms | 0.5837 ms |  0.78 |    0.01 | 1916.6667 |  750.0000 | 166.6667 |  11.55 MB |
    */

    [CoreJob, MemoryDiagnoser]
    public class GroupByCharCharChar : StringsBenchmarkBase
    {
        [Benchmark]
        public DesiredShape ForLoop()
        {
            var answer = new DesiredShape();

            foreach (var n in Words)
            {
                if (n.Length >= 3)
                {
                    var key = (n[0], n[1], n[2]);
                    if (!answer.TryGetValue(key, out var words))
                    {
                        words = new List<string>();
                        answer[key] = words;
                    }
                    words.Add(n);
                }
            }

            return answer;
        }

        [Benchmark(Baseline = true)]
        public DesiredShape SystemLinq()
        {
            return
                System.Linq.Enumerable.ToDictionary(
                    System.Linq.Enumerable.GroupBy(
                        System.Linq.Enumerable.Where(Words, w => w.Length >= 3),
                            w => (w[0], w[1], w[2])),
                                ws => ws.Key,
                                ws => System.Linq.Enumerable.ToList(ws));
        }

        [Benchmark]
        public DesiredShape CisternLinq()
        {
            return
                Words
                .Where(w => w.Length >= 3)
                .GroupBy(w => (w[0], w[1], w[2]))
                .ToDictionary(ws => ws.Key, ws => ws.ToList());
        }
    }
}
