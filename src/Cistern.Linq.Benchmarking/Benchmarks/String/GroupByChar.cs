using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |           Error |          StdDev |          Median | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------------:|----------------:|----------------:|----------------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 |        425.3 ns |       1.4121 ns |       1.3209 ns |        425.4 ns |  0.40 |    0.00 |    0.3133 |        - |        - |     984 B |
    |  SystemLinq |  False |         10 |      1,061.0 ns |       3.4144 ns |       3.0268 ns |      1,060.9 ns |  1.00 |    0.00 |    0.5665 |        - |        - |    1784 B |
    | CisternLinq |  False |         10 |      1,220.6 ns |       4.4765 ns |       3.7381 ns |      1,221.0 ns |  1.15 |    0.01 |    0.5188 |        - |        - |    1632 B |
    |             |        |            |                 |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 |     24,784.0 ns |     126.6984 ns |     118.5138 ns |     24,785.5 ns |  0.57 |    0.02 |   10.2539 |        - |        - |   32232 B |
    |  SystemLinq |  False |       1000 |     43,510.2 ns |     932.4197 ns |   1,583.3194 ns |     42,650.2 ns |  1.00 |    0.00 |   15.3809 |        - |        - |   48376 B |
    | CisternLinq |  False |       1000 |     38,062.5 ns |     123.5060 ns |     115.5276 ns |     38,049.2 ns |  0.87 |    0.03 |   10.8643 |        - |        - |   34192 B |
    |             |        |            |                 |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 35,091,126.5 ns | 368,925.4556 ns | 327,042.8070 ns | 35,130,925.0 ns |  0.78 |    0.02 | 1000.0000 | 571.4286 | 285.7143 | 4572417 B |
    |  SystemLinq |  False |     466544 | 44,950,475.9 ns | 873,288.6896 ns | 970,658.0646 ns | 45,105,400.0 ns |  1.00 |    0.00 | 1333.3333 | 916.6667 | 416.6667 | 5514901 B |
    | CisternLinq |  False |     466544 | 24,104,270.6 ns |  74,567.2007 ns |  69,750.2058 ns | 24,100,462.5 ns |  0.54 |    0.01 |  875.0000 | 625.0000 | 343.7500 | 3424914 B |
    |             |        |            |                 |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |         10 |        279.7 ns |       0.9317 ns |       0.7274 ns |        279.7 ns |  0.40 |    0.00 |    0.1783 |        - |        - |     560 B |
    |  SystemLinq |   True |         10 |        702.3 ns |       1.0864 ns |       0.9631 ns |        702.3 ns |  1.00 |    0.00 |    0.3672 |        - |        - |    1152 B |
    | CisternLinq |   True |         10 |        888.0 ns |       2.0021 ns |       1.7748 ns |        888.2 ns |  1.26 |    0.00 |    0.3719 |        - |        - |    1168 B |
    |             |        |            |                 |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 |     17,022.7 ns |      74.1479 ns |      69.3580 ns |     17,019.9 ns |  0.60 |    0.00 |    8.6365 |        - |        - |   27200 B |
    |  SystemLinq |   True |       1000 |     28,326.7 ns |     117.4650 ns |     104.1297 ns |     28,298.4 ns |  1.00 |    0.00 |   11.8103 |        - |        - |   37096 B |
    | CisternLinq |   True |       1000 |     25,282.2 ns |     116.6907 ns |     109.1526 ns |     25,259.2 ns |  0.89 |    0.01 |    9.9182 |        - |        - |   31184 B |
    |             |        |            |                 |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 13,426,141.5 ns | 263,151.9646 ns | 332,802.9775 ns | 13,530,493.8 ns |  0.76 |    0.03 | 1015.6250 | 578.1250 | 281.2500 | 4572712 B |
    |  SystemLinq |   True |     466544 | 17,727,869.1 ns | 354,322.9188 ns | 508,159.3425 ns | 17,721,253.1 ns |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 | 5514728 B |
    | CisternLinq |   True |     466544 | 10,702,113.6 ns | 169,092.0376 ns | 141,199.5096 ns | 10,640,809.4 ns |  0.61 |    0.03 |  656.2500 | 640.6250 | 328.1250 | 2101282 B |
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
