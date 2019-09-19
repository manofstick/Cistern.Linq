namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type UnfoldEnumerator<'State, 'T> =
    val f : 'State->option<'T*'State>
    val mutable state : 'State
    val mutable current : 'T
    val mutable running : bool

    new (f:'State->option<'T*'State>, seed:'State) = {
         f = f
         state = seed
         current = Unchecked.defaultof<_>
         running = true
    }

    interface IEnumerator<'T> with
        member this.Current: 'T = this.current
        member this.Current: obj = box this.current
        member this.MoveNext(): bool = 
            if this.running then
                match this.f this.state with
                | None -> this.running <- false
                | Some (c, s) ->
                    this.current <- c
                    this.state <- s
            this.running

        member __.Reset () = () 
        member __.Dispose(): unit = ()

[<Struct; NoComparison; NoEquality>]
type UnfoldEnumerable<'State, 'T>(f:'State->option<'T*'State>, seed:'State) =
    interface Optimizations.ITypedEnumerable<'T, UnfoldEnumerator<'State, 'T>> with
        member __.GetEnumerator () = new UnfoldEnumerator<'State, 'T>(f, seed)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false

