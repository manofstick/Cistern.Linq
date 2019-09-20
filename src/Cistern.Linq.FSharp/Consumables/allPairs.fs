namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type AllPairsEnumerator<'T1, 'T2> =
    val enumerator1 : IEnumerator<'T1>
    val enumerable2 : IEnumerable<'T2>
    
    val mutable running : bool
    val mutable enumerator2 : IEnumerator<'T2>
    val mutable current1 : 'T1
    val mutable current : 'T1*'T2

    new (enumerator1:IEnumerator<'T1>,enumerable2:IEnumerable<'T2>) = {
        enumerator1 = enumerator1
        enumerable2 = enumerable2

        running = true
        enumerator2 = Unchecked.defaultof<_>
        current1 = Unchecked.defaultof<_>
        current = Unchecked.defaultof<_>
    }

    member this.MoveNext() =
        if this.running then
            if isNull this.enumerator2 then
                this.running <- this.enumerator1.MoveNext ()
                if not this.running then
                    this.current1 <- Unchecked.defaultof<_>
                    this.current <- Unchecked.defaultof<_>
                else
                    this.current1 <- this.enumerator1.Current
                    this.enumerator2 <- this.enumerable2.GetEnumerator ()

            if this.running then
                let moveNext = this.enumerator2.MoveNext ()
                if not moveNext then
                    this.enumerator2 <- null
                    this.running <- this.MoveNext ()
                else
                    this.current <- (this.current1, this.enumerator2.Current)

        this.running

    interface IEnumerator<'T1*'T2> with
        member this.Current: 'T1*'T2 = this.current
        member this.Current: obj = box this.current
        member this.MoveNext(): bool = this.MoveNext ()

        member this.Reset () =
            this.enumerator1.Reset ()
            this.enumerator2.Reset ()

        member this.Dispose(): unit =
            if not (isNull this.enumerator1) then
                this.enumerator1.Dispose ()
            if not (isNull this.enumerator2) then
                this.enumerator2.Dispose ()

[<Struct; NoComparison; NoEquality>]
type AllPairsEnumerable<'T1, 'T2>(enumerable1:IEnumerable<'T1>, enumerable2:IEnumerable<'T2>) =
    interface Optimizations.ITypedEnumerable<'T1*'T2, AllPairsEnumerator<'T1, 'T2>> with
        member __.GetEnumerator () = new AllPairsEnumerator<'T1,'T2>(enumerable1.GetEnumerator (), enumerable2)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false

