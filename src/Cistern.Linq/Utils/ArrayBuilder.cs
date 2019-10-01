using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cistern.Linq
{
    struct ArrayBuilder<T>
    {
        const int InitialSize = 4;
        const int MaxChunkSize = 5000;

        T[] chunk;
        int chunkPos;
        List<T[]> chunks;
        int count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T t)
        {
            if (chunk == null)
            {
                Initialize();
            }
            else if (chunk.Length == chunkPos)
            {
                ActivateNewChunk();
            }

            chunk[chunkPos++] = t;
            count++;
        }

        private void ActivateNewChunk()
        {
            if (chunks == null)
            {
                chunks = new List<T[]>();
            }
            chunks.Add(chunk);
            chunk = new T[Math.Min(chunk.Length * 2, MaxChunkSize)];
            chunkPos = 0;
        }

        private void Initialize()
        {
            chunk = new T[InitialSize];
            chunkPos = 0;
            chunks = null;
            count = 0;
        }

        public T[] ToArray()
        {
            T[] result;

            if (count == 0)
            {
                result = Array.Empty<T>();
            }
            else if (chunks == null)
            {
                Array.Resize(ref chunk, count);
                result = chunk;
            }
            else
            {
                result = new T[count];
                var idx = 0;

                foreach (var chunk in chunks)
                {
                    chunk.CopyTo(result, idx);
                    idx += chunk.Length;
                }

                Array.Copy(chunk, 0, result, idx, chunkPos);
            }

            chunk = null;
            chunks = null;

            return result;
        }

    }
}
