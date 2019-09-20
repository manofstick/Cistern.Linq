namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type ChunkBySizeEnumerator<'T> =
    val enumerator : IEnumerator<'T>
    val chunkSize  : int

    val mutable running : bool
    val mutable complete : bool
    val mutable current : array<'T>

    new (enumerator:IEnumerator<'T>, chunkSize:int) = {
         enumerator = enumerator
         chunkSize = chunkSize

         running = true
         complete = false
         current = Unchecked.defaultof<_>
    }

    interface IEnumerator<array<'T>> with
        member this.Current: array<'T> = this.current
        member this.Current: obj = box this.current
        member this.MoveNext(): bool = 
            if this.running then
                if this.complete then
                    this.current <- Unchecked.defaultof<_>
                    this.running <- false
                else
                    this.current <- Array.zeroCreate this.chunkSize
                    let mutable idx = 0
                    while idx < this.current.Length do
                        if this.enumerator.MoveNext () then
                            this.current.[idx] <- this.enumerator.Current
                            idx <- idx + 1
                        elif idx = 0 then
                            this.current <- System.Array.Empty ()
                            this.running <- false
                        else
                            System.Array.Resize (&this.current, idx)
                            this.complete <- true
            this.running

        member this.Reset () = this.enumerator.Reset ()

        member this.Dispose(): unit =
            if not (isNull this.enumerator) then
                this.enumerator.Dispose ()

[<Struct; NoComparison; NoEquality>]
type ChunkBySizeEnumerable<'T>(enumerable:IEnumerable<'T>, chunkSize:int) =
    interface Optimizations.ITypedEnumerable<array<'T>, ChunkBySizeEnumerator<'T>> with
        member __.GetEnumerator () = new ChunkBySizeEnumerator<'T>(enumerable.GetEnumerator (), chunkSize)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false

