namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type PairwiseEnumerator<'T> =
    val enumerator : IEnumerator<'T>

    val mutable running : bool
    val mutable prior : ValueOption<'T>
    val mutable current : 'T*'T

    new (enumerator:IEnumerator<'T>) = {
         enumerator = enumerator

         running = true
         prior = ValueNone
         current = Unchecked.defaultof<_>
    }

    interface IEnumerator<'T*'T> with
        member this.Current: 'T*'T = this.current
        member this.Current: obj = box this.current
        member this.MoveNext(): bool = 
            if this.running then
                let prior = 
                    match this.prior with
                    | ValueNone ->
                        this.running <- this.enumerator.MoveNext ()
                        if not this.running then
                            Unchecked.defaultof<_>
                        else
                            this.enumerator.Current
                    | ValueSome value -> value

                if this.running then
                    this.running <- this.enumerator.MoveNext ()
                    if not this.running then
                        this.current <- Unchecked.defaultof<_>
                    else
                        let current = this.enumerator.Current
                        this.current <- (prior, current)
                        this.prior <- ValueSome current
            this.running

        member this.Reset () = this.enumerator.Reset ()

        member this.Dispose(): unit =
            if not (isNull this.enumerator) then
                this.enumerator.Dispose ()

[<Struct; NoComparison; NoEquality>]
type PairwiseEnumerable<'T>(enumerable:IEnumerable<'T>) =
    interface Optimizations.ITypedEnumerable<'T*'T, PairwiseEnumerator<'T>> with
        member __.GetEnumerator () = new PairwiseEnumerator<'T>(enumerable.GetEnumerator ())
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false

