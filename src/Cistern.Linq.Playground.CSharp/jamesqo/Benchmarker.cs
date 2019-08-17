namespace Playground.jamesko
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public static class Benchmarker
    {
        // The action should take long enough that an extra function invocation will not
        // have a measurable impact on the timings.
        public static ValueTuple<int, int> Bench(Action code) => Bench(state => state(), code);

        public static ValueTuple<int, int> Bench<T>(Action<T> code, T state)
        {
            // Time and measure GC allocs.
            var sw = new Stopwatch();
            int gen0 = GC.CollectionCount(0);
            sw.Start();

            code(state);

            sw.Stop();
            int millis = checked((int)sw.ElapsedMilliseconds);
            return ValueTuple.Create(millis, GC.CollectionCount(0) - gen0);
        }

        public static void Run(int times, Action code)
        {
            for (int i = 0; i < times; i++)
            {
                code();
            }
        }

        public static void Run<T>(IEnumerable<T> configs, Action<T> code)
        {
            foreach (T config in configs)
            {
                code(config);
            }
        }
    }
}
