using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |          Error |         StdDev |          Median | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 |        509.3 ns |       4.034 ns |       3.773 ns |        510.7 ns |  0.40 |    0.00 |    0.3128 |        - |        - |     984 B |
    |  SystemLinq |  False |         10 |      1,257.8 ns |       7.637 ns |       7.144 ns |      1,256.2 ns |  1.00 |    0.00 |    0.5665 |        - |        - |    1784 B |
    | CisternLinq |  False |         10 |      1,523.2 ns |       9.706 ns |       8.604 ns |      1,524.8 ns |  1.21 |    0.01 |    0.5417 |        - |        - |    1704 B |
    |             |        |            |                 |                |                |                 |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 |     28,500.0 ns |     252.329 ns |     236.029 ns |     28,529.5 ns |  0.57 |    0.01 |   10.2539 |        - |        - |   32232 B |
    |  SystemLinq |  False |       1000 |     49,922.3 ns |     199.530 ns |     186.640 ns |     49,897.9 ns |  1.00 |    0.00 |   15.3809 |        - |        - |   48376 B |
    | CisternLinq |  False |       1000 |     45,891.3 ns |     342.650 ns |     320.515 ns |     45,909.0 ns |  0.92 |    0.01 |   10.8643 |        - |        - |   34264 B |
    |             |        |            |                 |                |                |                 |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 41,418,380.0 ns | 208,185.705 ns | 194,737.038 ns | 41,389,538.5 ns |  0.88 |    0.00 |  923.0769 | 538.4615 | 230.7692 | 4571706 B |
    |  SystemLinq |  False |     466544 | 47,334,101.8 ns | 333,121.680 ns | 311,602.226 ns | 47,316,954.5 ns |  1.00 |    0.00 | 1363.6364 | 818.1818 | 454.5455 | 5515639 B |
    | CisternLinq |  False |     466544 | 27,366,416.9 ns | 192,716.749 ns | 180,267.367 ns | 27,353,334.4 ns |  0.58 |    0.00 |  875.0000 | 625.0000 | 343.7500 | 3425145 B |
    |             |        |            |                 |                |                |                 |       |         |           |          |          |           |
    |     ForLoop |   True |         10 |        323.4 ns |       2.231 ns |       1.978 ns |        323.4 ns |  0.39 |    0.00 |    0.1783 |        - |        - |     560 B |
    |  SystemLinq |   True |         10 |        821.1 ns |       4.105 ns |       3.839 ns |        822.1 ns |  1.00 |    0.00 |    0.3662 |        - |        - |    1152 B |
    | CisternLinq |   True |         10 |      1,186.3 ns |      26.511 ns |      29.467 ns |      1,184.3 ns |  1.45 |    0.04 |    0.3948 |        - |        - |    1240 B |
    |             |        |            |                 |                |                |                 |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 |     21,146.6 ns |     421.316 ns |     759.720 ns |     20,800.2 ns |  0.65 |    0.03 |    8.6365 |        - |        - |   27200 B |
    |  SystemLinq |   True |       1000 |     33,848.7 ns |     473.828 ns |     443.219 ns |     33,743.9 ns |  1.00 |    0.00 |   11.7798 |        - |        - |   37096 B |
    | CisternLinq |   True |       1000 |     33,522.4 ns |     647.870 ns |     864.888 ns |     33,581.3 ns |  0.98 |    0.02 |    9.9487 |        - |        - |   31256 B |
    |             |        |            |                 |                |                |                 |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 11,830,133.2 ns | 227,925.123 ns | 213,201.301 ns | 11,802,135.9 ns |  0.64 |    0.01 | 1015.6250 | 578.1250 | 281.2500 | 4572500 B |
    |  SystemLinq |   True |     466544 | 18,377,445.1 ns | 290,314.302 ns | 257,356.067 ns | 18,392,367.2 ns |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 | 5515466 B |
    | CisternLinq |   True |     466544 | 14,131,183.6 ns | 275,036.913 ns | 327,411.880 ns | 14,089,165.6 ns |  0.77 |    0.02 |  656.2500 | 640.6250 | 328.1250 | 2100767 B |
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
