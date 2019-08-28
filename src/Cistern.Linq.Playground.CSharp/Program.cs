using System;
using System.Diagnostics;
#if manofstick_test_chainlinq
using System.ChainLinq;
#else
using Cistern.Linq;
#endif

class Program
{
    public static void Main()
    {
        var sw = new Stopwatch();
        var rnd = new Random(08041988);

        foreach (var Count in new[] { 0, 10, 100, 10000, 1000000 })
        {
            var values = Enumerable.Range(1, Count).Select(x => rnd.NextDouble()).ToArray();

            var dim1 = values.Take(values.Length / 10).ToArray();
            var dim2 = values.Take(20).ToArray();

            sw.Restart();

            var sum = 0.0;

            for (var i = 0; i < 100000000 / (Count + 1); ++i)
            {
                sum += (from x in dim1
                        from y in dim2
                        select x * y).Sum();
            }

            var time = sw.ElapsedMilliseconds;

            Console.WriteLine("{0}\t{1}\t({2})", Count, time, sum);
        }
    }
}