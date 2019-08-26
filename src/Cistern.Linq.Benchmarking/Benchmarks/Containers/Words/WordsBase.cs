using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;

namespace Cistern.Linq.Benchmarking.Benchmarks.Containers.Words
{
    public abstract class WordsBase
	{
        const string source = "https://github.com/dwyl/english-words/raw/042fb7abca733b186f150dcce85023048e84705f/words.txt";
        const string filename = "lotsofwords.txt";

        [Params(ContainerType.Array, ContainerType.List, ContainerType.Enumerable, ContainerType.ImmutableArray, ContainerType.ImmutableList, ContainerType.FSharpList)]
        public ContainerType ContainerType;

        [Params(true, false)]
        public bool Sorted;

        [Params(10, 1000, 466544)]
		public int WordsCount;

		public IEnumerable<string> Words;

		[GlobalSetup]
		public void Setup()
		{
            Immutable.Register.RegisterSystemCollectionsImmutable();
            FSharp.Register.RegisterFSharpCollections();

            if (!System.IO.File.Exists(filename))
            {
                new System.Net.WebClient().DownloadFile(source, filename);
            }
            var words = System.IO.File.ReadAllLines(filename);

            if (Sorted)
            {
                Words = words.OrderBy(x => x).Take(WordsCount).ToContainer(ContainerType);
            }
            else
            {
                // deterministric "random" shuffle...
                var r = new Random(42);
                for (var i = 0; i < words.Length; ++i)
                {
                    var j = r.Next(i, words.Length - 1);
                    var tmp = words[i];
                    words[i] = words[j];
                    words[j] = tmp;
                }
                Words = words.Take(WordsCount).ToContainer(ContainerType);
            }
		}
	}
}
