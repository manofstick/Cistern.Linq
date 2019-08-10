namespace Playground.jamesko.SystemLinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // TODO: Add support for measuring GC allocations.
    public class PerfDataWriter
    {
        public PerfDataWriter()
        {
            bool isNew = Environment.GetEnvironmentVariable("DOTNET_TESTING_NEW") != null;
            string text = isNew ? "NEW" : "OLD";
            Console.WriteLine($"--- {text}");
        }

        public void WriteHeader(string text)
        {
            Console.WriteLine($"*** {text}");
        }

        public void WriteMillis(params int[] millis) => WriteMillis(millis.AsEnumerable());

        public void WriteMillis(IEnumerable<int> millis) => WriteTimesAndGcs(millis.Select(m => ValueTuple.Create(m, new int?())));

        public void WriteTimesAndGcs(IEnumerable<ValueTuple<int, int>> trials) => WriteTimesAndGcs(trials.Select(t => ValueTuple.Create(t.Item1, (int?)t.Item2)));

        public void WriteTimesAndGcs(params ValueTuple<int, int?>[] trials) => WriteTimesAndGcs(trials.AsEnumerable());

        public void WriteTimesAndGcs(IEnumerable<ValueTuple<int, int?>> trials)
        {
            // Data is encoded as @ Time1/Gc1,Time2/Gc2,..TimeN/GcN
            // If GC data is missing then it will just be @ Time1,Time2,Time3.. and
            // the data processor will exclude GCs from its analysis.
            var strings = trials.Select(t => t.Item1.ToString() + (t.Item2 == null ? "" : $"/{t.Item2}"));
            Console.WriteLine($"@ {string.Join(",", strings)}");
        }
    }
}


namespace Playground.jamesko.CisternLinq
{
    using System;
    using System.Collections.Generic;
    using Cistern.Linq;

    // TODO: Add support for measuring GC allocations.
    public class PerfDataWriter
    {
        public PerfDataWriter()
        {
            bool isNew = Environment.GetEnvironmentVariable("DOTNET_TESTING_NEW") != null;
            string text = isNew ? "NEW" : "OLD";
            Console.WriteLine($"--- {text}");
        }

        public void WriteHeader(string text)
        {
            Console.WriteLine($"*** {text}");
        }

        public void WriteMillis(params int[] millis) => WriteMillis(millis.AsEnumerable());

        public void WriteMillis(IEnumerable<int> millis) => WriteTimesAndGcs(millis.Select(m => ValueTuple.Create(m, new int?())));

        public void WriteTimesAndGcs(IEnumerable<ValueTuple<int, int>> trials) => WriteTimesAndGcs(trials.Select(t => ValueTuple.Create(t.Item1, (int?)t.Item2)));

        public void WriteTimesAndGcs(params ValueTuple<int, int?>[] trials) => WriteTimesAndGcs(trials.AsEnumerable());

        public void WriteTimesAndGcs(IEnumerable<ValueTuple<int, int?>> trials)
        {
            // Data is encoded as @ Time1/Gc1,Time2/Gc2,..TimeN/GcN
            // If GC data is missing then it will just be @ Time1,Time2,Time3.. and
            // the data processor will exclude GCs from its analysis.
            var strings = trials.Select(t => t.Item1.ToString() + (t.Item2 == null ? "" : $"/{t.Item2}"));
            Console.WriteLine($"@ {string.Join(",", strings)}");
        }
    }
}
