using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

using DesiredShape = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;

namespace Cistern.Linq.Benchmarking.Benchmarks.String
{
    /*
    |      Method | Sorted | WordsCount |            Mean |           Error |          StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
    |------------ |------- |----------- |----------------:|----------------:|----------------:|------:|--------:|----------:|---------:|---------:|----------:|
    |     ForLoop |  False |         10 |        428.3 ns |       1.7276 ns |       1.6160 ns |  0.40 |    0.00 |    0.3133 |        - |        - |     984 B |
    |  SystemLinq |  False |         10 |      1,066.3 ns |       3.3933 ns |       3.0080 ns |  1.00 |    0.00 |    0.5665 |        - |        - |    1784 B |
    | CisternLinq |  False |         10 |      1,357.0 ns |       4.4550 ns |       4.1672 ns |  1.27 |    0.00 |    0.6237 |        - |        - |    1960 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |       1000 |     24,480.8 ns |     137.1272 ns |     128.2689 ns |  0.58 |    0.00 |   10.2539 |        - |        - |   32232 B |
    |  SystemLinq |  False |       1000 |     42,480.8 ns |     140.8597 ns |     109.9740 ns |  1.00 |    0.00 |   15.3809 |        - |        - |   48376 B |
    | CisternLinq |  False |       1000 |     40,335.8 ns |     133.2308 ns |     124.6242 ns |  0.95 |    0.00 |   11.4136 |        - |        - |   35880 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |  False |     466544 | 35,333,818.9 ns | 568,581.5302 ns | 504,032.7167 ns |  0.78 |    0.02 | 1000.0000 | 571.4286 | 285.7143 | 4572581 B |
    |  SystemLinq |  False |     466544 | 45,075,973.9 ns | 719,779.1735 ns | 673,281.8855 ns |  1.00 |    0.00 | 1333.3333 | 916.6667 | 416.6667 | 5515499 B |
    | CisternLinq |  False |     466544 | 26,261,420.2 ns | 127,493.2304 ns | 119,257.2468 ns |  0.58 |    0.01 |  906.2500 | 656.2500 | 375.0000 | 3427128 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |         10 |        280.3 ns |       0.9580 ns |       0.8961 ns |  0.40 |    0.00 |    0.1783 |        - |        - |     560 B |
    |  SystemLinq |   True |         10 |        704.8 ns |       3.0939 ns |       2.4155 ns |  1.00 |    0.00 |    0.3672 |        - |        - |    1152 B |
    | CisternLinq |   True |         10 |        974.1 ns |       4.0455 ns |       3.7842 ns |  1.38 |    0.01 |    0.4311 |        - |        - |    1360 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |       1000 |     17,080.3 ns |      71.9495 ns |      67.3016 ns |  0.61 |    0.01 |    8.6365 |        - |        - |   27200 B |
    |  SystemLinq |   True |       1000 |     27,880.0 ns |     232.5337 ns |     217.5122 ns |  1.00 |    0.00 |   11.8103 |        - |        - |   37096 B |
    | CisternLinq |   True |       1000 |     26,120.3 ns |      64.0819 ns |      56.8070 ns |  0.94 |    0.01 |   10.1318 |        - |        - |   31800 B |
    |             |        |            |                 |                 |                 |       |         |           |          |          |           |
    |     ForLoop |   True |     466544 | 13,531,006.0 ns | 224,778.4863 ns | 210,257.9356 ns |  0.80 |    0.03 | 1015.6250 | 593.7500 | 281.2500 | 4572914 B |
    |  SystemLinq |   True |     466544 | 16,872,597.7 ns | 334,770.0567 ns | 586,322.7495 ns |  1.00 |    0.00 | 1281.2500 | 812.5000 | 406.2500 | 5515504 B |
    | CisternLinq |   True |     466544 | 11,574,344.0 ns | 166,844.3655 ns | 156,066.3230 ns |  0.69 |    0.03 |  656.2500 | 640.6250 | 328.1250 | 2102775 B |
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
