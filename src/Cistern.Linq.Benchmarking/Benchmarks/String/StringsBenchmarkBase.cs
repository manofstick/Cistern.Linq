using BenchmarkDotNet.Attributes;
using System;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    public abstract class StringsBenchmarkBase
	{
        const string source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
        const string filename = "lotsofwords.txt";

        [Params(true, false)]
        public bool Sorted;

        [Params(10, 1000, 466544)]
		public int WordsCount;

		public string[] Words;

		[GlobalSetup]
		public void Setup()
		{
            if (!System.IO.File.Exists(filename))
            {
                new System.Net.WebClient().DownloadFile(source, filename);
            }
            Words = System.IO.File.ReadAllLines(filename);

            if (Sorted)
            {
                Words = Words.OrderBy(x => x).ToArray();
            }
            else
            {
                // deterministric "random" shuffle...
                var r = new Random(42);
                for (var i = 0; i < Words.Length; ++i)
                {
                    var j = r.Next(i, Words.Length - 1);
                    var tmp = Words[i];
                    Words[i] = Words[j];
                    Words[j] = tmp;
                }
            }

            Words.Take(WordsCount).ToArray();
		}
	}
}
