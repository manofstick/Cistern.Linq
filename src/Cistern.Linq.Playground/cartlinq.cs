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

// double?
cartlinqCisternLinq
0       7890    (0)
10      2153    (31020883.135855)
100     1773    (32648189.4854866)
10000   1621    (43570280.597243)
1000000 1595    (46208168.7086638)

0       8093    (0)
10      2062    (31020883.135855)
100     1387    (32648189.4854866)
10000   1224    (43570280.597243)
1000000 1216    (46208168.7086638)

// Decimal
cartlinqSystemLinq
0       1148    (0.0)
10      1980    (3102085.2432815948029133632449)
100     5326    (3264789.2712955124290330493239)
10000   6070    (4353106.3422982745248294739873)
1000000 5532    (4200742.6098785592962801467986)

cartlinqCisternLinq
0       1328    (0.0)
10      1877    (3102085.2432815948029133632449)
100     4921    (3264789.2712955124290330493239)
10000   5760    (4353106.3422982745248294739873)
1000000 5218    (4200742.6098785592962801467986)

float


cartlinqCisternLinq
0       7450    (0)
10      1802    (3.229762E+07)
100     943     (3.219111E+07)
10000   793     (4.356814E+07)
1000000 779     (4.620816E+07)
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
                var values = Enumerable.Range(1, Count).Select(x => (float)rnd.NextDouble()).ToArray();

                var dim1 = values.Take(values.Length / 10).ToArray();
                var dim2 = values.Take(20).ToArray();

                sw.Restart();

                float sum = 0.0f;

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
                var values = Enumerable.Range(1, Count).Select(x => (float)rnd.NextDouble()).ToArray();

                var dim1 = values.Take(values.Length / 10).ToArray();
                var dim2 = values.Take(20).ToArray();

                sw.Restart();

                float sum = 0.0f;

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
