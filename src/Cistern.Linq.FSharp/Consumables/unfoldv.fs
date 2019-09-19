namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type UnfoldVEnumerator<'State, 'T> =
    val f : 'State->voption<'T*'State>
    val mutable state : 'State
    val mutable current : 'T
    val mutable running : bool

    new (f:'State->voption<'T*'State>, seed:'State) = {
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
                | ValueNone -> this.running <- false
                | ValueSome (c, s) ->
                    this.current <- c
                    this.state <- s
            this.running

        member __.Reset () = () 
        member __.Dispose(): unit = ()

[<Struct; NoComparison; NoEquality>]
type UnfoldVEnumerable<'State, 'T>(f:'State->voption<'T*'State>, seed:'State) =
    interface Optimizations.ITypedEnumerable<'T, UnfoldVEnumerator<'State, 'T>> with
        member __.GetEnumerator () = new UnfoldVEnumerator<'State, 'T>(f, seed)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast _ = false
