
namespace Playground.groupBy.CisternLinq
{
    using System;
    using Cistern.Linq;
    using System.Diagnostics;

    class Program
    {
        const string source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
        const string filename = "lotsofwords.txt";

        public static void groupby()
        {
            Console.WriteLine($"Environment.Is64BitProcess={Environment.Is64BitProcess}");

            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("...downloading words...");
                new System.Net.WebClient().DownloadFile(source, filename);
            }
            var words = System.IO.File.ReadAllLines(filename);

            var sw = new Stopwatch();

            var iterations = 10;
            var totalTime = 0L;

            for (var i = 0; i < iterations; ++i)
            {
                sw.Restart();

                for (var j = 0; j < 100; ++j)
                {
                    var byFirstLetter =
                        words
                        .GroupBy(x => x[0])
                        .ToDictionary(kv => kv.Key, kv => kv.Count());

                    if (byFirstLetter['t'] != 21218)
                        throw new Exception("bad word file...");
                }

                var elapsed = sw.ElapsedMilliseconds;
                totalTime += elapsed;

                Console.WriteLine(elapsed);
            }

            Console.WriteLine($"average={totalTime / iterations}");
        }
    }
}

namespace Playground.groupBy.SystemLinq
{
    using System;
    using System.Linq;
    using System.Diagnostics;

    class Program
    {
        const string source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
        const string filename = "lotsofwords.txt";

        public static void groupby()
        {
            Console.WriteLine($"Environment.Is64BitProcess={Environment.Is64BitProcess}");

            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("...downloading words...");
                new System.Net.WebClient().DownloadFile(source, filename);
            }
            var words = System.IO.File.ReadAllLines(filename);

            var sw = new Stopwatch();

            var iterations = 10;
            var totalTime = 0L;

            for (var i = 0; i < iterations; ++i)
            {
                sw.Restart();

                for (var j = 0; j < 100; ++j)
                {
                    var byFirstLetter =
                        words
                        .GroupBy(x => x[0])
                        .ToDictionary(kv => kv.Key, kv => kv.Count());

                    if (byFirstLetter['t'] != 21218)
                        throw new Exception("bad word file...");
                }

                var elapsed = sw.ElapsedMilliseconds;
                totalTime += elapsed;

                Console.WriteLine(elapsed);
            }

            Console.WriteLine($"average={totalTime / iterations}");
        }
    }
}