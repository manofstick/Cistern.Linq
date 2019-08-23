using BenchmarkDotNet.Attributes;

namespace Cistern.Linq.Benchmarking.Benchmarks
{
    public abstract class StringsBenchmarkBase
	{
        const string source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
        const string filename = "lotsofwords.txt";

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

            Words.Take(WordsCount).ToArray();
		}
	}
}
