namespace Cistern.Linq.FSharp

open Cistern.Linq
open Cistern.Linq.ChainLinq
open Cistern.Linq.FSharp
open System
open System.Runtime.CompilerServices

module Linq =
    [<Literal>]
    let internal exceptionSource = "Cistern.Linq"

    let allPairs (source1:seq<'T1>) (source2:seq<'T2>) : seq<'T1*'T2> =
        if isNull source1 then
            ThrowHelper.ThrowArgumentNullException ExceptionArgument.source1
        if isNull source2 then
            ThrowHelper.ThrowArgumentNullException ExceptionArgument.source2

        upcast Consumables.Enumerable(Consumables.AllPairsEnumerable (source1, source2), Links.Identity.Instance)

    let append (source1:seq<'T>) (source2:seq<'T>) : seq<'T> = source1.Concat source2

    let chunkBySize (chunkSize:int) (source:seq<'T>) : seq<array<'T>> = 
        if chunkSize <= 0 then
            ThrowHelper.ThrowArgumentOutOfRangeException ExceptionArgument.chunkSize
        if isNull source then
            ThrowHelper.ThrowArgumentNullException ExceptionArgument.source

        upcast Consumables.Enumerable(Consumables.ChunkBySizeEnumerable (source, chunkSize), Links.Identity.Instance)

    let collect (f:'T->#seq<'U>) (e:seq<'T>) : seq<'U> =
        if isNull e then
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

        let selectMany = ChainLinq.Utils.Select (e, fun x -> f x);
        ChainLinq.Consumables.SelectMany<_,_,_> (selectMany, ChainLinq.Links.Identity<_>.Instance) :> seq<'U>

    let concat (sources:seq<#seq<'Collection>>) : seq<'Collection> =
        if isNull sources then
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

        let sources = sources |> Cistern.Linq.ChainLinq.Utils.AsConsumable 
        upcast ChainLinq.Consumables.SelectMany<_,_,_> (sources, ChainLinq.Links.Identity<_>.Instance)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let empty<'T> : seq<'T> = upcast Consumables.Empty<'T>.Instance

    let inline exists (f:'a->bool) (e:seq<'a>) = e.Any (fun x -> f x)

    let inline filter (f:'a->bool) (e:seq<'a>) : seq<'a> = e.Where f

    let inline find (predicate:'T->bool) (source:seq<'T>) = try source.First (fun x -> predicate x) with :? InvalidOperationException as e when e.Source = exceptionSource -> raise (System.Collections.Generic.KeyNotFoundException("An index satisfying the predicate was not found in the collection.", e))

    let inline fold (f:'s->'a->'s) seed (e:seq<'a>) = e.Aggregate (seed, fun a c -> f a c)

    let inline forall (f:'a->bool) (e:seq<'a>) = e.All (fun x -> f x)

    let head (e:seq<'a>) = try e.First () with :? InvalidOperationException as e when e.Source = exceptionSource -> raise (ArgumentException(e.Message, e))

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let init (count:int) (initializer:int->'T) : seq<'T> =
        if count < 0 then raise (ArgumentException ())
        elif count = 0 then upcast Consumables.Empty<'T>.Instance
        else upcast Consumables.Enumerable (Consumables.InitEnumerable(count, initializer), Links.Identity.Instance)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let initInfinite (initializer:int->'T) : seq<'T> =
        upcast Consumables.Enumerable (Consumables.InitEnumerable(Int32.MaxValue, initializer), Links.Identity.Instance)

    let isEmpty (e:seq<'a>) = not (e.Any ())

    let last (source:seq<'T>) : 'T =  try source.Last () with :? InvalidOperationException as e when e.Source = exceptionSource  -> raise (ArgumentException(e.Message, e))

    let length (e:seq<'a>) = e.Count ()

    let inline map (f:'a->'b) (e:seq<'a>) = e.Select f

    let inline mapi (f:int->'a->'b) (e:seq<'a>) = e.Select (fun a idx -> f idx a)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let ofArray (source:array<'T>) : seq<'T> = upcast Consumables.Array(source, 0, source.Length, Links.Identity.Instance)
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let ofList (source:list<'T>) : seq<'T> = upcast Consumables.Enumerable(TypedEnumerables.FSharpListEnumerable<_> source, Links.Identity.Instance)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let pairwise (source:seq<'T>) : seq<'T * 'T> = upcast Consumables.Enumerable(Consumables.PairwiseEnumerable source, Links.Identity.Instance)

    let inline reduce (f:'a->'a->'a) (e:seq<'a>) = try e.Aggregate (fun a c -> f a c) with :? InvalidOperationException as e when e.Source = exceptionSource  -> raise (ArgumentException(e.Message, e))

    let take count (e:seq<'a>) = e.Take count

    let inline takeWhile (f:'a->bool) (e:seq<'a>) = e.TakeWhile f

    let inline takeWhilei (f:int->'a->bool) (e:seq<'a>) = e.TakeWhile (fun a idx -> f idx a)

    let toArray (source:seq<'T>) : 'T[] = source.ToArray ()

    let toList (source:seq<'T>) : 'T list = List.ofSeq source

    let truncate (count:int) (source:seq<'T>) : seq<'T> = source.Take count

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let unfold (f:'State->option<'T*'State>) (seed:'State) : seq<'T> = Consumables.Enumerable (Consumables.UnfoldEnumerable(f, seed), Links.Identity.Instance) :> seq<'T>

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let unfoldV (f:'State->voption<'T*'State>) (seed:'State) : seq<'T> = Consumables.Enumerable (Consumables.UnfoldVEnumerable(f, seed), Links.Identity.Instance) :> seq<'T>

    let inline where (predicate:'T->bool) (source:seq<'T>) : seq<'T> = source.Where predicate

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let zip (source1:seq<'T1>) (source2:seq<'T2>) : seq<'T1*'T2> =
        if isNull source1 then
            ThrowHelper.ThrowArgumentNullException ExceptionArgument.source1
        if isNull source2 then
            ThrowHelper.ThrowArgumentNullException ExceptionArgument.source2

        upcast Consumables.Enumerable(Consumables.ZipEnumerable (source1, source2), Links.Identity.Instance)

type Linq =
    static member distinct (source:seq<'T>) : seq<'T> = source.Distinct HashIdentity.Structural

    static member sum (e:seq<float>)                   = e.Sum ()
    static member sum (e:seq<float32>)                 = e.Sum ()
    static member sum (e:seq<decimal>)                 = e.Sum ()
    static member sum (e:seq<int>)                     = e.Sum ()
    static member sum (e:seq<int64>)                   = e.Sum ()
    static member sum (e:seq<Nullable<float>>)         = e.Sum ()
    static member sum (e:seq<Nullable<float32>>)       = e.Sum ()
    static member sum (e:seq<Nullable<decimal>>)       = e.Sum ()
    static member sum (e:seq<Nullable<int>>)           = e.Sum ()
    static member sum (e:seq<Nullable<int64>>)         = e.Sum ()
    static member inline sum (source:seq<(^T)>) = Seq.sum source

    static member tryLinqGenericSum (source:seq<('T)>) : ValueOption<'T> = 
        if   obj.ReferenceEquals(typeof<'T>, typeof<float>)             then unbox<seq<float>> source               |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<float32>)           then unbox<seq<float32>> source             |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<decimal>)           then unbox<seq<decimal>> source             |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<int>)               then unbox<seq<int>> source                 |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<int64>)             then unbox<seq<int64>> source               |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<float>>)   then unbox<seq<Nullable<float>>> source     |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<float32>>) then unbox<seq<Nullable<float32>>> source   |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<decimal>>) then unbox<seq<Nullable<decimal>>> source   |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<int>>)     then unbox<seq<Nullable<int>>> source       |> Linq.sum |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<int64>>)   then unbox<seq<Nullable<int64>>> source     |> Linq.sum |> unbox<'T> |> ValueSome
        else ValueNone

    static member inline sumBy (projection:'T -> ^U) (source:seq<'T>) : ^U =
        match source |> Linq.map projection |> Linq.tryLinqGenericSum with
        | ValueSome sum -> sum
        | ValueNone -> Seq.sumBy projection source
                                                              
    static member average (e:seq<float>)               = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<float32>)             = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<decimal>)             = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<int>)                 = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<int64>)               = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<Nullable<float>>)     = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<Nullable<float32>>)   = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<Nullable<decimal>>)   = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<Nullable<int>>)       = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member average (e:seq<Nullable<int64>>)     = try e.Average () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member inline average (source:seq<(^T)>) : 'T = Seq.average source
    
    static member tryLinqGenericAverage (source:seq<('T)>) : ValueOption<'T> = 
        if   obj.ReferenceEquals(typeof<'T>, typeof<float>)             then unbox<seq<float>> source               |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<float32>)           then unbox<seq<float32>> source             |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<decimal>)           then unbox<seq<decimal>> source             |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<int>)               then unbox<seq<int>> source                 |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<int64>)             then unbox<seq<int64>> source               |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<float>>)   then unbox<seq<Nullable<float>>> source     |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<float32>>) then unbox<seq<Nullable<float32>>> source   |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<decimal>>) then unbox<seq<Nullable<decimal>>> source   |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<int>>)     then unbox<seq<Nullable<int>>> source       |> Linq.average |> unbox<'T> |> ValueSome
        elif obj.ReferenceEquals(typeof<'T>, typeof<Nullable<int64>>)   then unbox<seq<Nullable<int64>>> source     |> Linq.average |> unbox<'T> |> ValueSome
        else ValueNone

    static member inline averageBy (projection:'T -> ^U) (source:seq<'T>) : 'U =
        match source |> Linq.map projection |> Linq.tryLinqGenericAverage with
        | ValueSome average -> average
        | ValueNone -> Seq.averageBy projection source
                                                              
    static member min (e:seq<float>)                   = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<float32>)                 = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<decimal>)                 = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<int>)                     = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<int64>)                   = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<Nullable<float>>)         = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<Nullable<float32>>)       = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<Nullable<decimal>>)       = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<Nullable<int>>)           = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member min (e:seq<Nullable<int64>>)         = try e.Min () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member inline min (e:seq<'T>) = Seq.min e
            
    static member max (e:seq<float>)                   = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<decimal>)                 = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<int>)                     = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<int64>)                   = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<float32>)                 = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<Nullable<float>>)         = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<Nullable<float32>>)       = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<Nullable<decimal>>)       = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<Nullable<int>>)           = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member max (e:seq<Nullable<int64>>)         = try e.Max () with :? InvalidOperationException as e when e.Source = Linq.exceptionSource -> raise (ArgumentException(e.Message, e))
    static member inline max (e:seq<'T>) = Seq.max e

    // polyfill
    static member inline minBy (projection:'T->'U) (source:seq<'T>) = Seq.minBy projection source
    static member inline maxBy (projection:'T->'U) (source:seq<'T>) : 'T = Seq.maxBy projection source

    static member inline cache (source:seq<'T>) : seq<'T>= Seq.cache source
    static member inline cast (source:System.Collections.IEnumerable) : seq<'T> = Seq.cast source
    static member inline choose (chooser:'T -> 'U option) (source:seq<'T>) : seq<'U> = Seq.choose chooser source
    static member inline compareWith (comparer:'T->'T->int) (source1:seq<'T>) (source2:seq<'T>) : int = Seq.compareWith comparer source1 source2
    static member inline contains (value:'T) (source:seq<'T>) : bool = Seq.contains value source
    static member inline countBy (projection:'T->'Key) (source:seq<'T>) : seq<'Key*int> = Seq.countBy projection source
    static member inline delay (generator:unit->seq<'T>) : seq<'T> = Seq.delay generator
    static member inline distinctBy (projection:'T->'Key) (source:seq<'T>) : seq<'T> = Seq.distinctBy projection source
    static member inline splitInto (count:int) (source:seq<'T>) : seq<array<'T>> = Seq.splitInto count source
    static member inline except (itemsToExclude:seq<'T>) (source:seq<'T>) : seq<'T> = Seq.except itemsToExclude source
    static member inline exists2 (predicate:'T1->'T2->bool) (source1:seq<'T1>) (source2:seq<'T2>) : bool = Seq.exists2 predicate source1 source2
    
    static member inline findBack (predicate:'T->bool) (source:seq<'T>) = Seq.findBack predicate source
    static member inline findIndex (predicate:'T->bool) (source:seq<'T>) = Seq.findIndex predicate source
    static member inline findIndexBack (predicate:'T->bool) (source:seq<'T>) = Seq.findIndexBack predicate source
    static member inline fold2<'T1,'T2,'State> (folder:'State->'T1->'T2->'State) (state:'State) (source1:seq<'T1>) (source2:seq<'T2>) = Seq.fold2 folder state source1 source2
    static member inline foldBack<'T,'State> (folder:'T->'State->'State) (source:seq<'T>) (state:'State) = Seq.foldBack folder source state
    static member inline foldBack2<'T1,'T2,'State> (folder:'T1->'T2->'State->'State) (source1:seq<'T1>) (source2:seq<'T2>) (state:'State) = Seq.foldBack2 folder source1 source2 state
    static member inline forall2 (predicate:'T1->'T2->bool) (source1:seq<'T1>) (source2:seq<'T2>) = Seq.forall2 predicate source1 source2
    static member inline groupBy (projection:'T->'Key) (source:seq<'T>) = Seq.groupBy projection source
    static member inline tryHead (source:seq<'T>) : option<'T> = Seq.tryHead source
    static member inline tryLast (source:seq<'T>) : option<'T> = Seq.tryLast source
    static member inline exactlyOne (source:seq<'T>) : 'T = Seq.exactlyOne source
    static member inline tryExactlyOne (source:seq<'T>) : option<'T> = Seq.tryExactlyOne source
    static member inline indexed (source:seq<'T>) : seq<int*'T> = Seq.indexed source
    static member inline item (index:int) (source:seq<'T>) : 'T = Seq.item index source
    static member inline iter (action:'T->unit) (source:seq<'T>) : unit = Seq.iter action source
    static member inline iteri (action:int->'T->unit) (source:seq<'T>) : unit = Seq.iteri action source
    static member inline iter2 (action:'T1->'T2->unit) (source1:seq<'T1>) (source2:seq<'T2>) : unit = Seq.iter2 action source1 source2
    static member inline iteri2 (action:int->'T1->'T2->unit) (source1:seq<'T1>) (source2:seq<'T2>) : unit = Seq.iteri2 action source1 source2
    static member inline map2 (mapping:'T1->'T2->'U) (source1:seq<'T1>) (source2:seq<'T2>) : seq<'U> = Seq.map2 mapping source1 source2
    static member inline mapFold (mapping:'State->'T->'Result*'State) (state:'State) (source:seq<'T>) : seq<'Result>*'State = Seq.mapFold mapping state source
    static member inline mapFoldBack (mapping:'T->'State->'Result*'State) (source:seq<'T>) (state:'State) : seq<'Result> * 'State = Seq.mapFoldBack mapping source state
    static member inline map3 (mapping:'T1->'T2->'T3->'U) (source1:seq<'T1>) (source2:seq<'T2>) (source3:seq<'T3>) : seq<'U> = Seq.map3 mapping source1 source2 source3
    static member inline mapi2 (mapping:int->'T1->'T2->'U) (source1:seq<'T1>) (source2:seq<'T2>) : seq<'U> = Seq.mapi2 mapping source1 source2
    static member inline permute (indexMap:int->int) (source:seq<'T>) : seq<'T> = Seq.permute indexMap source
    static member inline pick (chooser:'T->'U option) (source:seq<'T>) : 'U = Seq.pick chooser source
    static member inline readonly (source:seq<'T>) : seq<'T> = Seq.readonly source
    static member inline replicate (count:int) (initial:'T) : seq<'T> = Seq.replicate count initial
    static member inline reduceBack (reduction:'T->'T->'T) (source:seq<'T>) : 'T = Seq.reduceBack reduction source
    static member inline rev (source:seq<'T>) : seq<'T> = Seq.rev source
    static member inline scan (folder:'State->'T->'State) (state:'State) (source:seq<'T>) : seq<'State> = Seq.scan folder state source
    static member inline scanBack<'T,'State> (folder:'T->'State->'State) (source:seq<'T>) (state:'State) : seq<'State> = Seq.scanBack folder source state
    static member inline singleton (value:'T) : seq<'T> = Seq.singleton value
    static member inline skip (count:int) (source:seq<'T>) : seq<'T> = Seq.skip count source
    static member inline skipWhile (predicate:'T->bool) (source:seq<'T>) : seq<'T> = Seq.skipWhile predicate source
    static member inline sort (source:seq<'T>) : seq<'T>  = Seq.sort source
    static member inline sortWith (comparer:'T->'T->int) (source:seq<'T>) : seq<'T> = Seq.sortWith comparer source
    static member inline sortBy (projection:'T->'Key) (source:seq<'T>) : seq<'T> = Seq.sortBy projection source
    static member inline sortDescending (source:seq<'T>) : seq<'T> = Seq.sortDescending source
    static member inline sortByDescending (projection:'T->'Key) (source:seq<'T>) : seq<'T> = Seq.sortByDescending projection source
    static member inline tail (source:seq<'T>) : seq<'T> = Seq.tail source
    static member inline tryFind (predicate:'T->bool) (source:seq<'T>) : 'T option = Seq.tryFind predicate source
    static member inline tryFindBack (predicate:'T->bool) (source:seq<'T>) : 'T option = Seq.tryFindBack predicate source
    static member inline tryFindIndex  (predicate:'T->bool) (source:seq<'T>) : int option = Seq.tryFindIndex predicate source
    static member inline tryItem (index:int) (source:seq<'T>) : 'T option = Seq.tryItem index source
    static member inline tryFindIndexBack  (predicate:'T->bool) (source:seq<'T>) : int option = Seq.tryFindIndexBack predicate source
    static member inline tryPick (chooser:'T -> 'U option) (source:seq<'T>) : 'U option = Seq.tryPick chooser source
    static member inline transpose (source:seq<'Collection>) : seq<seq<'T>> = Seq.transpose source
    static member inline windowed (windowSize:int) (source:seq<'T>) : seq<'T[]> = Seq.windowed windowSize source
    static member inline zip3 (source1:seq<'T1>) (source2:seq<'T2>) (source3:seq<'T3>) : seq<'T1 * 'T2 * 'T3> = Seq.zip3 source1 source2 source3
