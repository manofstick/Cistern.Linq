using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |          Error |         StdDev | Ratio |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------------:|---------------:|---------------:|------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 |        512.4 ns |       5.400 ns |       4.787 ns |  0.41 |    0.3128 |        - |        - |     984 B |
    |  SystemLinq |  False |         10 |      1,242.9 ns |       9.023 ns |       8.440 ns |  1.00 |    0.5665 |        - |        - |    1784 B |
    | CisternLinq |  False |         10 |      1,498.6 ns |      14.056 ns |      11.737 ns |  1.21 |    0.5417 |        - |        - |    1704 B |
    |             |        |            |                 |                |                |       |           |          |          |           |
    |     ForLoop |  False |       1000 |     29,274.1 ns |     187.955 ns |     175.813 ns |  0.60 |   10.2539 |        - |        - |   32232 B |
    |  SystemLinq |  False |       1000 |     49,111.4 ns |     295.519 ns |     261.970 ns |  1.00 |   15.3809 |        - |        - |   48376 B |
    | CisternLinq |  False |       1000 |     42,546.9 ns |     286.613 ns |     268.098 ns |  0.87 |   10.8643 |        - |        - |   34264 B |
    |             |        |            |                 |                |                |       |           |          |          |           |
    |     ForLoop |  False |     466544 | 41,296,451.3 ns | 338,836.745 ns | 316,948.101 ns |  0.88 |  923.0769 | 538.4615 | 230.7692 | 4572282 B |
    |  SystemLinq |  False |     466544 | 47,011,104.2 ns | 498,483.502 ns | 466,281.777 ns |  1.00 | 1363.6364 | 818.1818 | 454.5455 | 5515716 B |
    | CisternLinq |  False |     466544 | 26,654,268.3 ns | 146,273.084 ns | 136,823.933 ns |  0.57 |  906.2500 | 656.2500 | 375.0000 | 3425154 B |
    |             |        |            |                 |                |                |       |           |          |          |           |
    |     ForLoop |   True |         10 |        328.4 ns |       3.309 ns |       2.933 ns |  0.40 |    0.1783 |        - |        - |     560 B |
    |  SystemLinq |   True |         10 |        831.1 ns |       5.346 ns |       5.000 ns |  1.00 |    0.3672 |        - |        - |    1152 B |
    | CisternLinq |   True |         10 |      1,065.6 ns |       6.412 ns |       5.355 ns |  1.28 |    0.3948 |        - |        - |    1240 B |
    |             |        |            |                 |                |                |       |           |          |          |           |
    |     ForLoop |   True |       1000 |     19,827.7 ns |     103.301 ns |      96.627 ns |  0.61 |    8.6365 |        - |        - |   27200 B |
    |  SystemLinq |   True |       1000 |     32,431.7 ns |     182.280 ns |     170.504 ns |  1.00 |   11.7798 |        - |        - |   37096 B |
    | CisternLinq |   True |       1000 |     27,417.6 ns |     194.585 ns |     182.015 ns |  0.85 |    9.9487 |        - |        - |   31256 B |
    |             |        |            |                 |                |                |       |           |          |          |           |
    |     ForLoop |   True |     466544 | 11,503,574.7 ns | 120,218.709 ns | 112,452.655 ns |  0.65 | 1000.0000 | 562.5000 | 265.6250 | 4572422 B |
    |  SystemLinq |   True |     466544 | 17,781,893.3 ns | 186,838.808 ns | 174,769.137 ns |  1.00 | 1281.2500 | 812.5000 | 406.2500 | 5515324 B |
    | CisternLinq |   True |     466544 | 11,861,250.8 ns | 198,437.909 ns | 185,618.943 ns |  0.67 |  656.2500 | 640.6250 | 328.1250 | 2101146 B |
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
