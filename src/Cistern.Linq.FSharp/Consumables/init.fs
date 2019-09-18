namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open Cistern.Linq.ChainLinq.Consumables
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type InitEnumerator<'State, 'T> =
    val count           : int
    val initializer     : int->'T
    val mutable index   : int
    val mutable current : 'T

    new (count:int, initializer:int->'T) = {
         count = count
         initializer = initializer
         index = 0
         current = Unchecked.defaultof<_>
    }

    interface IEnumerator<'T> with
        member this.Current : 'T = this.current
        member this.Current : obj = box this.current
        member this.MoveNext (): bool = 
            if this.index >= this.count then false
            else
                this.current <- this.initializer this.index
                this.index <- this.index + 1
                true

        member __.Reset () = () 
        member __.Dispose(): unit = ()

[<Struct; NoComparison; NoEquality>]
type InitEnumerable<'State, 'T>(count:int, initializer:int->'T) =
    interface Optimizations.ITypedEnumerable<'T, InitEnumerator<'State, 'T>> with
        member __.GetEnumerator () = new InitEnumerator<'State, 'T>(count, initializer)
        member __.Source = Unchecked.defaultof<_>
        member __.TryLength = System.Nullable count
        member __.TryGetSourceAsSpan _ = false
        member __.TryLast result = 
            result <- initializer (count-1)
            true

[<Sealed>]
type Init<'State, 'T, 'V>(count:int, initializer:int->'T, link:ILink<'T, 'V>) =
    inherit Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<'V, 'T>(link)

    override __.Create    (first:ILink<'T, 'V>) = Init<'State, 'T, 'V>(count, initializer, first) :> Consumable<'V>
    override __.Create<'W>(first:ILink<'T, 'W>) = Init<'State, 'T, 'W>(count, initializer, first) :> Consumable<'W>

    override this.TailLink = if base.IsIdentity then box this else base.TailLink

    interface Optimizations.IMergeSelect<'V> with
        member __.MergeSelect<'W> (_,selector:System.Func<'V,'W>) = 
            Unchecked.unbox (new SelectEnumerable<_,_,_,'W>(InitEnumerable(count, initializer), Unchecked.unbox selector))

    interface Optimizations.IMergeWhere<'V> with
        member __.MergeWhere (_,selector) = 
            Unchecked.unbox (new WhereEnumerable<_,_,'T>(InitEnumerable(count, initializer), Unchecked.unbox selector))

    override __.GetEnumerator () =
        upcast new ConsumerEnumerators.Enumerable<InitEnumerable<'State, 'T>, InitEnumerator<'State, 'T>, 'T, 'V>(new InitEnumerable<'State, 'T>(count, initializer), link);
    
    override __.Consume(consumer:Consumer<'V> ) =
        let chain = link.Compose consumer
        try
            match box chain with
            | :? Optimizations.IHeadStart<'T> as optimized -> 
                optimized.Execute<InitEnumerable<'State, 'T>, InitEnumerator<'State, 'T>>(new InitEnumerable<'State, 'T>(count, initializer))
                |> ignore
            | _ ->
                let rec iterate index =
                    if index >= count then ()
                    else
                        let state = chain.ProcessNext (initializer index)
                        if not (ProcessNextResultHelper.IsStopped state) then
                            iterate (index+1)

                iterate 0

            chain.ChainComplete();
        finally
            chain.ChainDispose();


