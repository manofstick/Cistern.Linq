/* from https://github.com/dotnet/coreclr/issues/2385#issuecomment-166740124

SystemLinq version:
sum=4001980000000 time=4663.0147
sum=4001980000000 time=27520.5946

CisternLinq version:
sum=4001980000000 time=4759.6308
sum=4001980000000 time=8742.4558

*/

namespace Cistern.Linq.Playground.mikedn.SystemLinq
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    class Program
    {
        const int size = 2000;
        const int iterations = 2000000;
        static double[] numbers;

        static double ForLoopSum(double[] numbers)
        {
            double sum = 0;
            foreach (var n in numbers)
            {
                if (n >= 5.0)
                    sum += n;
            }
            return sum;
        }

        static void TestForLoopSum()
        {
            var sum = 0.0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                sum += ForLoopSum(numbers);

            sw.Stop();
            Console.WriteLine("sum={0} time={1}", sum, sw.Elapsed.TotalMilliseconds);
        }

        static double LinqSum(double[] numbers)
        {
            return numbers.Where(n => n >= 5.0).Sum();
        }

        static void TestLinqSum()
        {
            var sum = 0.0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                sum += LinqSum(numbers);

            sw.Stop();
            Console.WriteLine("sum={0} time={1}", sum, sw.Elapsed.TotalMilliseconds);
        }

        public static void mikedn()
        {
            numbers = Enumerable.Range(1, size).Select(i => (double)i).ToArray();

            ForLoopSum(numbers);
            LinqSum(numbers);

            TestForLoopSum();
            TestLinqSum();
        }
    }
}

namespace Cistern.Linq.Playground.mikedn.CisternLinq
{
    using System;
    using System.Diagnostics;
    using Cistern.Linq;

    class Program
    {
        const int size = 2000;
        const int iterations = 2000000;
        static double[] numbers;

        static double ForLoopSum(double[] numbers)
        {
            double sum = 0;
            foreach (var n in numbers)
            {
                if (n >= 5.0)
                    sum += n;
            }
            return sum;
        }

        static void TestForLoopSum()
        {
            var sum = 0.0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                sum += ForLoopSum(numbers);

            sw.Stop();
            Console.WriteLine("sum={0} time={1}", sum, sw.Elapsed.TotalMilliseconds);
        }

        static double LinqSum(double[] numbers)
        {
            return numbers.Where(n => n >= 5.0).Sum();
        }

        static void TestLinqSum()
        {
            var sum = 0.0;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                sum += LinqSum(numbers);

            sw.Stop();
            Console.WriteLine("sum={0} time={1}", sum, sw.Elapsed.TotalMilliseconds);
        }

        public static void mikedn()
        {
            numbers = Enumerable.Range(1, size).Select(i => (double)i).ToArray();

            ForLoopSum(numbers);
            LinqSum(numbers);

            TestForLoopSum();
            TestLinqSum();
        }
    }
}
