namespace Cistern.Linq.FSharp

open Cistern.Linq
open Cistern.Linq.ChainLinq
open Cistern.Linq.FSharp
open System
open System.Runtime.CompilerServices

type Linq =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member unfold (f:'State->option<'T*'State>) (seed:'State) : seq<'T> = Consumables.Unfold (f, seed, Links.Identity.Instance) :> seq<'T>
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member unfoldV (f:'State->voption<'T*'State>) (seed:'State) : seq<'T> = Consumables.UnfoldV (f, seed, Links.Identity.Instance) :> seq<'T>

    static member inline map (f:'a->'b) (e:seq<'a>)           = e.Select f
    static member inline mapi (f:int->'a->'b) (e:seq<'a>)     = e.Select (fun a idx -> f idx a)
                                                              
    static member inline filter (f:'a->bool) (e:seq<'a>)      : seq<'a> = e.Where f
    static member inline where (predicate:'T->bool) (source:seq<'T>) : seq<'T> = source.Where predicate
                                                              
    static member inline reduce (f:'a->'a->'a) (e:seq<'a>)    = e.Aggregate (fun a c -> f a c)

    static member inline fold (f:'s->'a->'s) seed (e:seq<'a>) = e.Aggregate (seed, fun a c -> f a c)

    static member inline take count (e:seq<'a>)               = e.Take count
    static member inline takeWhile (f:'a->bool) (e:seq<'a>)   = e.TakeWhile f
    static member inline takeWhilei (f:int->'a->bool) (e:seq<'a>) = e.TakeWhile (fun a idx -> f idx a)

    static member inline forall (f:'a->bool) (e:seq<'a>)      = e.All (fun x -> f x)
    static member inline exists (f:'a->bool) (e:seq<'a>)      = e.Any (fun x -> f x)
    static member inline head (e:seq<'a>)                     = e.First ()
    static member inline isEmpty (e:seq<'a>)                  = not (e.Any ())

    static member collect (f:'T->#seq<'U>) (e:seq<'T>) : seq<'U> =
        if isNull e then
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

        if isNull e then
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);

        let selectMany = ChainLinq.Utils.Select (e, fun x -> f x);
        ChainLinq.Consumables.SelectMany<_,_,_> (selectMany, ChainLinq.Links.Identity<_>.Instance) :> seq<'U>

    static member inline length (e:seq<'a>)                   = e.Count ()

    static member inline sum (e:seq<float>)                   = e.Sum ()
    static member inline sum (e:seq<float32>)                 = e.Sum ()
    static member inline sum (e:seq<decimal>)                 = e.Sum ()
    static member inline sum (e:seq<int>)                     = e.Sum ()
    static member inline sum (e:seq<int64>)                   = e.Sum ()
    static member inline sum (e:seq<Nullable<float>>)         = e.Sum ()
    static member inline sum (e:seq<Nullable<float32>>)       = e.Sum ()
    static member inline sum (e:seq<Nullable<decimal>>)       = e.Sum ()
    static member inline sum (e:seq<Nullable<int>>)           = e.Sum ()
    static member inline sum (e:seq<Nullable<int64>>)         = e.Sum ()
                                                              
    static member inline min (e:seq<float>)                   = e.Min ()
    static member inline min (e:seq<float32>)                 = e.Min ()
    static member inline min (e:seq<decimal>)                 = e.Min ()
    static member inline min (e:seq<int>)                     = e.Min ()
    static member inline min (e:seq<int64>)                   = e.Min ()
    static member inline min (e:seq<Nullable<float>>)         = e.Min ()
    static member inline min (e:seq<Nullable<float32>>)       = e.Min ()
    static member inline min (e:seq<Nullable<decimal>>)       = e.Min ()
    static member inline min (e:seq<Nullable<int>>)           = e.Min ()
    static member inline min (e:seq<Nullable<int64>>)         = e.Min ()
                                                              
    static member inline max (e:seq<float>)                   = e.Max ()
    static member inline max (e:seq<float32>)                 = e.Max ()
    static member inline max (e:seq<decimal>)                 = e.Max ()
    static member inline max (e:seq<int>)                     = e.Max ()
    static member inline max (e:seq<int64>)                   = e.Max ()
    static member inline max (e:seq<Nullable<float>>)         = e.Max ()
    static member inline max (e:seq<Nullable<float32>>)       = e.Max ()
    static member inline max (e:seq<Nullable<decimal>>)       = e.Max ()
    static member inline max (e:seq<Nullable<int>>)           = e.Max ()
    static member inline max (e:seq<Nullable<int64>>)         = e.Max ()

    // polyfill

    static member inline allPairs (source1:seq<'T1>) (source2:seq<'T2>) : seq<'T1*'T2> = Seq.allPairs source1 source2
    static member inline append (source1:seq<'T>) (source2:seq<'T>) : seq<'T> = Seq.append source1 source2
    static member inline average (source:seq<(^T)>) : 'T = Seq.average source 
    static member inline averageBy (projection:'T -> ^U) (source:seq<'T>) : 'U = Seq.averageBy projection source
    static member inline cache (source:seq<'T>) : seq<'T>= Seq.cache source
    static member inline cast (source:System.Collections.IEnumerable) : seq<'T> = Seq.cast source
    static member inline choose (chooser:'T -> 'U option) (source:seq<'T>) : seq<'U> = Seq.choose chooser source
    static member inline chunkBySize (chunkSize:int) (source:seq<'T>) : seq<array<'T>> = Seq.chunkBySize chunkSize source
    static member inline compareWith (comparer:'T->'T->int) (source1:seq<'T>) (source2:seq<'T>) : int = Seq.compareWith comparer source1 source2
    static member inline concat (sources:#seq<'Collection>) : seq<'Collection> = Seq.concat sources
    static member inline contains (value:'T) (source:seq<'T>) : bool = Seq.contains value source
    static member inline countBy (projection:'T->'Key) (source:seq<'T>) : seq<'Key*int> = Seq.countBy projection source
    static member inline delay (generator:unit->seq<'T>) : seq<'T> = Seq.delay generator
    static member inline distinct (source:seq<'T>) : seq<'T> = Seq.distinct source
    static member inline distinctBy (projection:'T->'Key) (source:seq<'T>) : seq<'T> = Seq.distinctBy projection source
    static member inline splitInto (count:int) (source:seq<'T>) : seq<array<'T>> = Seq.splitInto count source
    static member inline empty () : seq<'T> = Seq.empty<'T>
    static member inline except (itemsToExclude:seq<'T>) (source:seq<'T>) : seq<'T> = Seq.except itemsToExclude source
    static member inline exists2 (predicate:'T1->'T2->bool) (source1:seq<'T1>) (source2:seq<'T2>) : bool = Seq.exists2 predicate source1 source2
    static member inline find (predicate:'T->bool) (source:seq<'T>) = Seq.find predicate source
    static member inline findBack (predicate:'T->bool) (source:seq<'T>) = Seq.findBack predicate source
    static member inline findIndex (predicate:'T->bool) (source:seq<'T>) = Seq.findIndex predicate source
    static member inline findIndexBack (predicate:'T->bool) (source:seq<'T>) = Seq.findIndexBack predicate source
    static member inline fold2<'T1,'T2,'State> (folder:'State->'T1->'T2->'State) (state:'State) (source1:seq<'T1>) (source2:seq<'T2>) = Seq.fold2 folder state source1 source2
    static member inline foldBack<'T,'State> (folder:'T->'State->'State) (source:seq<'T>) (state:'State) = Seq.foldBack folder source state
    static member inline foldBack2<'T1,'T2,'State> (folder:'T1->'T2->'State->'State) (source1:seq<'T1>) (source2:seq<'T2>) (state:'State) = Seq.foldBack2 folder source1 source2 state
    static member inline forall2 (predicate:'T1->'T2->bool) (source1:seq<'T1>) (source2:seq<'T2>) = Seq.forall2 predicate source1 source2
    static member inline groupBy (projection:'T->'Key) (source:seq<'T>) = Seq.groupBy projection source
    static member inline tryHead (source:seq<'T>) : option<'T> = Seq.tryHead source
    static member inline last (source:seq<'T>) : 'T = Seq.last source
    static member inline tryLast (source:seq<'T>) : option<'T> = Seq.tryLast source
    static member inline exactlyOne (source:seq<'T>) : 'T = Seq.exactlyOne source
    static member inline tryExactlyOne (source:seq<'T>) : option<'T> = Seq.tryExactlyOne source
    static member inline indexed (source:seq<'T>) : seq<int*'T> = Seq.indexed source
    static member inline init (count:int) (initializer:int->'T) : seq<'T> = Seq.init count initializer
    static member inline initInfinite (initializer:int->'T) : seq<'T> = Seq.initInfinite initializer
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
    static member inline max   (source:seq<'T>) : 'T = Seq.max source
    static member inline maxBy (projection:'T->'U) (source:seq<'T>) : 'T = Seq.maxBy projection source
    static member inline min   (source:seq<'T>) : 'T = Seq.min source
    static member inline minBy (projection:'T->'U) (source:seq<'T>) = Seq.minBy projection source
    static member inline ofArray (source:array<'T>) : seq<'T> = Seq.ofArray source
    static member inline ofList (source:list<'T>) : seq<'T> = Seq.ofList source
    static member inline pairwise (source:seq<'T>) : seq<'T * 'T> = Seq.pairwise source
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
    static member inline sum (source:seq<(^T)>) = Seq.sum source
    static member inline sumBy (projection:'T -> ^U) (source:seq<'T>) : ^U = Seq.sumBy projection source
    static member inline tail (source:seq<'T>) : seq<'T> = Seq.tail source
    static member inline toArray (source:seq<'T>) : 'T[] = Seq.toArray source
    static member inline toList (source:seq<'T>) : 'T list = Seq.toList source
    static member inline tryFind (predicate:'T->bool) (source:seq<'T>) : 'T option = Seq.tryFind predicate source
    static member inline tryFindBack (predicate:'T->bool) (source:seq<'T>) : 'T option = Seq.tryFindBack predicate source
    static member inline tryFindIndex  (predicate:'T->bool) (source:seq<'T>) : int option = Seq.tryFindIndex predicate source
    static member inline tryItem (index:int) (source:seq<'T>) : 'T option = Seq.tryItem index source
    static member inline tryFindIndexBack  (predicate:'T->bool) (source:seq<'T>) : int option = Seq.tryFindIndexBack predicate source
    static member inline tryPick (chooser:'T -> 'U option) (source:seq<'T>) : 'U option = Seq.tryPick chooser source
    static member inline transpose (source:seq<'Collection>) : seq<seq<'T>> = Seq.transpose source
    static member inline truncate (count:int) (source:seq<'T>) : seq<'T> = Seq.truncate count source
    static member inline windowed (windowSize:int) (source:seq<'T>) : seq<'T[]> = Seq.windowed windowSize source
    static member inline zip (source1:seq<'T1>) (source2:seq<'T2>) : seq<'T1*'T2> = Seq.zip source1 source2
    static member inline zip3 (source1:seq<'T1>) (source2:seq<'T2>) (source3:seq<'T3>) : seq<'T1 * 'T2 * 'T3> = Seq.zip3 source1 source2 source3
