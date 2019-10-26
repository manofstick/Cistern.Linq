using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cistern.Linq;
using Cistern.Linq.Benchmarking.Benchmarks.Numeric;
using Cistern.Linq.Benchmarking.Vanilla.Enumerable;
using Microsoft.FSharp.Collections;

namespace Playground
{
    enum Playthings
    {
        benchmark,

        system_mikedn,
        cistern_mikedn,
        system_groupby,
        cistern_groupby,
        system_mikedn_immutable,
        cistern_mikedn_immutable,
        system_cartlinq,
        cistern_cartlinq,
        system_jamesqo,
        cistern_jamesqo,
        system_matthewwatson,
        cistern_matthewwatson,
    }

    class Program
    {
        static Playthings plaything = Playthings.benchmark;

        static IEnumerable<int> F(IEnumerable<int> x)
        {
#if true
            return x;
#else
            foreach (var y in x)
                yield return y;
#endif
        }

        static void Benchmark()
        {
#if true
            var z = new Cistern.Linq.Benchmarking.Benchmarks.OrderBy.OrderBy_ByString();

            z.CustomerCount = 1000;
            z.PreSorted = Cistern.Linq.Benchmarking.Benchmarks.OrderBy.SortOrder.Forward;

            z.Setup();

            for (var j = 0; j < 10; ++j)
            {
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < 1000; ++i)
                {
                    var count = z.CisternLinq();
                    //var count = z.SystemLinq();
                }
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
#endif
#if trueX
            var data = ListModule.OfSeq(Enumerable.Range(0, 10000));
            Cistern.Linq.FSharp.Register.RegisterFSharpCollections();

            for (var j = 0; j < 10; ++j)
            {
                var sw = Stopwatch.StartNew();
                for (var i = 0; i < 10000; ++i)
                {
                    var zz = data.Select(x => x).ToList();
                    
                }
                Console.WriteLine(sw.ElapsedMilliseconds);
            }


#endif

            //var x = F(new[] { 1, 2, 3 });
            //var y = F(new[] { 4, 5, 6 });
            //var z = F(new[] { 7, 8, 9 });

            //var a = x.Concat(y).Concat(z);

            //var b = a.Where(m => m < 6).Last();

            //Console.WriteLine(b);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.benchmark: Benchmark(); break;

                case Playthings.cistern_mikedn: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.system_mikedn: mikedn.SystemLinq.Program.mikedn(); break;

                case Playthings.cistern_groupby: groupBy.CisternLinq.Program.groupby(); break;
                case Playthings.system_groupby: groupBy.SystemLinq.Program.groupby(); break;

                case Playthings.cistern_matthewwatson: matthewwatson.CisternLinq.Demo.Program.matthewwatson(); break;
                case Playthings.system_matthewwatson: matthewwatson.SystemLinq.Demo.Program.matthewwatson(); break;

                case Playthings.cistern_mikedn_immutable: mikedn_immutable.CisternLinq.Program.mikedn_immutable(); break;
                case Playthings.system_mikedn_immutable: mikedn_immutable.SystemLinq.Program.mikedn_immutable(); break;

                case Playthings.cistern_cartlinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.system_cartlinq: cartlinq.SystemLinq.Program.cartlinq(); break;

                case Playthings.cistern_jamesqo: jamesko.CisternLinq.Program.jamesqo(); break;
                case Playthings.system_jamesqo: jamesko.SystemLinq.Program.jamesqo(); break;
            }
        }
    }
}
