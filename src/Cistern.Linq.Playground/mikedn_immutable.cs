/* from https://github.com/dotnet/coreclr/issues/2385#issuecomment-166740124

*/

using TheCollection = Microsoft.FSharp.Collections.FSharpList<double>;

namespace Playground.mikedn_immutable.SystemLinq
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Immutable;

    class Program
    {
        const int size = 2000;
        const int iterations = 200000;
        static TheCollection numbers;

        static double ForLoopSum(TheCollection numbers)
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

        static double LinqSum(TheCollection numbers)
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

        public static void mikedn_immutable()
        {
            numbers = Microsoft.FSharp.Collections.ListModule.OfSeq(Enumerable.Range(1, size).Select(i => (double)i));

            ForLoopSum(numbers);
            LinqSum(numbers);

            TestForLoopSum();
            TestLinqSum();
        }
    }
}

namespace Playground.mikedn_immutable.CisternLinq
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Cistern.Linq;

    class Program
    {
        const int size = 2000;
        const int iterations = 200000;
        static TheCollection numbers;

        static double ForLoopSum(TheCollection numbers)
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

        static double LinqSum(TheCollection numbers)
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

        public static void mikedn_immutable()
        {
            //Cistern.Linq.Immutable.Register.RegisterSystemCollectionsImmutable();
            Cistern.Linq.FSharp.Register.RegisterFSharpCollections();

            numbers = Microsoft.FSharp.Collections.ListModule.OfSeq(Enumerable.Range(1, size).Select(i => (double)i));

            ForLoopSum(numbers);
            LinqSum(numbers);

            TestForLoopSum();
            TestLinqSum();
        }
    }
}
