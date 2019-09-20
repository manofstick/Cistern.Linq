namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type ZipEnumerator<'T1, 'T2> =
    val enumerator1 : IEnumerator<'T1>
    val enumerator2 : IEnumerator<'T2>

    val mutable running : bool
    val mutable current : 'T1*'T2

    new (enumerator1:IEnumerator<'T1>,enumerator2:IEnumerator<'T2>) = {
        enumerator1 = enumerator1
        enumerator2 = enumerator2

        running = true
        current = Unchecked.defaultof<_>
    }

    interface IEnumerator<'T1*'T2> with
        member this.Current: 'T1*'T2 = this.current
        member this.Current: obj = box this.current
        member this.MoveNext(): bool = 
            if this.running then
                this.running <- this.enumerator1.MoveNext() && this.enumerator2.MoveNext()
                this.current <- 
                    if not this.running
                    then Unchecked.defaultof<_>
                    else (this.enumerator1.Current, this.enumerator2.Current)
            this.running

        member this.Reset () =
            this.enumerator1.Reset ()
            this.enumerator2.Reset ()

        member this.Dispose(): unit =
            if not (isNull this.enumerator1) then
                this.enumerator1.Dispose ()
            if not (isNull this.enumerator2) then
                this.enumerator2.Dispose ()

[<Struct; NoComparison; NoEquality>]
type ZipEnumerable<'T1, 'T2>(enumerable1:IEnumerable<'T1>, enumerable2:IEnumerable<'T2>) =
    interface Optimizations.ITypedEnumerable<'T1*'T2, ZipEnumerator<'T1, 'T2>> with
        member __.GetEnumerator () = new ZipEnumerator<'T1,'T2>(enumerable1.GetEnumerator (), enumerable2.GetEnumerator ())
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false

