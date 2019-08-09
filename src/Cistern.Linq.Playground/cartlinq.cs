/*

SystemLinq version:
0       4689    (0)
10      1893    (31020883.135855)
100     2795    (32648189.4854866)
10000   2789    (43570280.597243)
1000000 2813    (46208168.7086638)

CisternLinq version:
0       6625    (0)
10      1575    (31020883.135855)
100     910     (32648189.4854866)
10000   780     (43570280.597243)
1000000 776     (46208168.7086638)

*/

namespace Cistern.Linq.Playground.cartlinq.SystemLinq
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    class Program
    {
        public static void cartlinq()
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
}

namespace Cistern.Linq.Playground.cartlinq.CisternLinq
{
    using System;
    using System.Diagnostics;
    using Cistern.Linq;

    class Program
    {
        public static void cartlinq()
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
}
