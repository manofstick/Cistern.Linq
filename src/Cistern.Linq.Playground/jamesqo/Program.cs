/*
system_jamesqo
...
Total time=20797

cistern_jamesqo
...
Total time = 13882
*/
namespace Playground.jamesko.SystemLinq
{
    using System;
    using System.Linq;

    public class Program
    {
        public static void jamesqo_Main()
        {
            var writer = new PerfDataWriter();
            var message = new MessageBuilder();

            Benchmarker.Run(new[] { 1, 2, 3, 5, 8, 13 }, sourceLength =>
            {
                Benchmarker.Run(new[] { 1, 2, 3, 5, 8, 13, 21, 34 }, subCollectionLength =>
                {
                    var results = Enumerable.Range(1, 5).Select(_ => RunBenchmark(sourceLength, subCollectionLength));

                    message["SourceLength"] = sourceLength;
                    message["SubCollectionLength"] = subCollectionLength;

                    writer.WriteHeader(message.ToStringAndClear());
                    writer.WriteTimesAndGcs(results);
                });
            });
        }

        public static void jamesqo()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            jamesqo_Main();
            System.Console.WriteLine($"Total time={sw.ElapsedMilliseconds}");
        }

        private static ValueTuple<int, int> RunBenchmark(int sourceLength, int subCollectionLength)
        {
            const int N = 100000;

            var source = Enumerable.Range(1, sourceLength).ToArray();
            var subCollection = Enumerable.Range(1, subCollectionLength).ToArray(); // This gets repeated `sourceLength` times.

            var iterator = source.SelectMany(_ => subCollection);

            return Benchmarker.Bench(state =>
            {
                for (int i = 0; i < N; i++)
                {
                    state.ToArray();
                }
            }, iterator);
        }
    }
}

namespace Playground.jamesko.CisternLinq
{
    using System;
    using Cistern.Linq;

    public class Program
    {
        public static void jamesqo_Main()
        {
            var writer = new PerfDataWriter();
            var message = new MessageBuilder();

            Benchmarker.Run(new[] { 1, 2, 3, 5, 8, 13 }, sourceLength =>
            {
                Benchmarker.Run(new[] { 1, 2, 3, 5, 8, 13, 21, 34 }, subCollectionLength =>
                {
                    var results = Enumerable.Range(1, 5).Select(_ => RunBenchmark(sourceLength, subCollectionLength));

                    message["SourceLength"] = sourceLength;
                    message["SubCollectionLength"] = subCollectionLength;

                    writer.WriteHeader(message.ToStringAndClear());
                    writer.WriteTimesAndGcs(results);
                });
            });
        }

        public static void jamesqo()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            jamesqo_Main();
            System.Console.WriteLine($"Total time={sw.ElapsedMilliseconds}");
        }

        private static ValueTuple<int, int> RunBenchmark(int sourceLength, int subCollectionLength)
        {
            const int N = 100000;

            var source = Enumerable.Range(1, sourceLength).ToArray();
            var subCollection = Enumerable.Range(1, subCollectionLength).ToArray(); // This gets repeated `sourceLength` times.

            var iterator = source.SelectMany(_ => subCollection);

            return Benchmarker.Bench(state =>
            {
                for (int i = 0; i < N; i++)
                {
                    state.ToArray();
                }
            }, iterator);
        }
    }
}