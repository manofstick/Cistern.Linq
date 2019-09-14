namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open Cistern.Linq.ChainLinq.Consumables
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
    interface Optimizations.ITypedEnumerable<'T, UnfoldVEnumerable<'State, 'T>, UnfoldVEnumerator<'State, 'T>> with
        member __.GetEnumerator () = new UnfoldVEnumerator<'State, 'T>(f, seed)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable ()
        member __.TryGetSourceAsSpan _ = false
        member __.TrySkip (_,_) = false

[<Sealed>]
type UnfoldV<'State, 'T, 'V>(f:'State->voption<'T*'State>, seed:'State, link:ILink<'T, 'V>) =
    inherit Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<'V, 'T>(link)

    override __.Create    (first:ILink<'T, 'V>) = UnfoldV<'State, 'T, 'V>(f, seed, first) :> Consumable<'V>
    override __.Create<'W>(first:ILink<'T, 'W>) = UnfoldV<'State, 'T, 'W>(f, seed, first) :> Consumable<'W>

    override this.TailLink = if base.IsIdentity then box this else base.TailLink

    interface Optimizations.IMergeSelect<'V> with
        member __.MergeSelect<'W> (_,selector:System.Func<'V,'W>) = 
            Unchecked.unbox (new SelectEnumerable<_,_,_,'W>(UnfoldVEnumerable(f, seed), Unchecked.unbox selector))

    interface Optimizations.IMergeWhere<'V> with
        member __.MergeWhere (_,selector) = 
            Unchecked.unbox (new WhereEnumerable<_,_,'T>(UnfoldVEnumerable(f, seed), Unchecked.unbox selector))

    override __.GetEnumerator () =
        upcast new ConsumerEnumerators.Enumerable<UnfoldVEnumerable<'State, 'T>, UnfoldVEnumerator<'State, 'T>, 'T, 'V>(new UnfoldVEnumerable<'State, 'T>(f, seed), link);
    
    override __.Consume(consumer:Consumer<'V> ) =
        let chain = link.Compose consumer
        try
            match box chain with
            | :? Optimizations.IHeadStart<'T> as optimized -> 
                optimized.Execute<UnfoldVEnumerable<'State, 'T>, UnfoldVEnumerator<'State, 'T>>(new UnfoldVEnumerable<'State, 'T>(f, seed))
                |> ignore
            | _ ->
                let rec iterate state =
                    match f state with
                    | ValueNone -> ()
                    | ValueSome (item, next) ->
                        let state = chain.ProcessNext item
                        if not (ProcessNextResultHelper.IsStopped state) then
                            iterate next

                iterate seed

            chain.ChainComplete();
        finally
            chain.ChainDispose();


