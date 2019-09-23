using Microsoft.FSharp.Collections;
using System;
using System.Runtime.CompilerServices;

namespace Cistern.Linq.Utils
{
    struct FSharpListBuilder<T>
    {
        const int InitialSize = 4;
        const int MaxChunkSize = 5000;

        T[] chunk;
        int chunkPos;
        FSharpList<T[]> chunks;

        public FSharpListBuilder(bool _)
        {
            chunk = Array.Empty<T>();
            chunkPos = 0;
            chunks = FSharpList<T[]>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T t)
        {
            if (chunk.Length == chunkPos)
            {
                ActivateNewChunk();
            }

            chunk[chunkPos++] = t;
        }

        private void ActivateNewChunk()
        {
            chunks = FSharpList<T[]>.Cons(chunk, chunks);
            chunk = new T[Math.Max(InitialSize, Math.Min(chunk.Length * 2, MaxChunkSize))];
            chunkPos = 0;
        }
        public FSharpList<T> ToFSharpList()
        {
            FSharpList<T> result = FSharpList<T>.Empty;

            result = CopyToList(result, chunk, chunkPos);

            while (!chunks.IsEmpty)
            {
                chunk = chunks.HeadOrDefault;
                chunks = chunks.TailOrNull;
                result = CopyToList(result, chunk, chunk.Length);
            }

            chunk = Array.Empty<T>();

            return result;
        }

        private FSharpList<T> CopyToList(FSharpList<T> result, T[] chunk, int length)
        {
            for (var i = length - 1; i >= 0; --i)
                result = FSharpList<T>.Cons(chunk[i], result);
            return result;
        }
    }
}
