namespace Playground.matthewwatson.SystemLinq
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    namespace Demo
    {
        public static class Program
        {
            public static void matthewwatson()
            {
                string[] a = new string[1000000];

                for (int i = 0; i < a.Length; ++i)
                {
                    a[i] = "Won't be found";
                }

                string matchString = "Will be found";

                a[a.Length - 1] = "Will be found";

                const int COUNT = 100;

                var sw = Stopwatch.StartNew();
                int matchIndex = -1;

                for (int outer = 0; outer < COUNT; ++outer)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == matchString)
                        {
                            matchIndex = i;
                            break;
                        }
                    }
                }

                sw.Stop();
                Console.WriteLine("Found via loop at index " + matchIndex + " in " + sw.Elapsed);
                double loopTime = sw.Elapsed.TotalSeconds;

                sw.Restart();

                for (int outer = 0; outer < COUNT; ++outer)
                {
                    matchIndex = a.Select((r, i) => new { value = r, index = i })
                                 .Where(t => t.value == matchString)
                                 .Select(s => s.index).First();
                }

                sw.Stop();
                Console.WriteLine("Found via linq at index " + matchIndex + " in " + sw.Elapsed);
                double linqTime = sw.Elapsed.TotalSeconds;

                Console.WriteLine("Loop was {0} times faster than linq.", linqTime / loopTime);
            }
        }
    }
}


namespace Playground.matthewwatson.CisternLinq
{
    using System;
    using System.Diagnostics;
    using Cistern.Linq;

    namespace Demo
    {
        public static class Program
        {
            public static void matthewwatson()
            {
                string[] a = new string[1000000];

                for (int i = 0; i < a.Length; ++i)
                {
                    a[i] = "Won't be found";
                }

                string matchString = "Will be found";

                a[a.Length - 1] = "Will be found";

                const int COUNT = 100;

                var sw = Stopwatch.StartNew();
                int matchIndex = -1;

                for (int outer = 0; outer < COUNT; ++outer)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == matchString)
                        {
                            matchIndex = i;
                            break;
                        }
                    }
                }

                sw.Stop();
                Console.WriteLine("Found via loop at index " + matchIndex + " in " + sw.Elapsed);
                double loopTime = sw.Elapsed.TotalSeconds;

                sw.Restart();

                for (int outer = 0; outer < COUNT; ++outer)
                {
                    matchIndex = a.Select((r, i) => new { value = r, index = i })
                                 .Where(t => t.value == matchString)
                                 .Select(s => s.index).First();
                }

                sw.Stop();
                Console.WriteLine("Found via linq at index " + matchIndex + " in " + sw.Elapsed);
                double linqTime = sw.Elapsed.TotalSeconds;

                Console.WriteLine("Loop was {0} times faster than linq.", linqTime / loopTime);
            }
        }
    }
}