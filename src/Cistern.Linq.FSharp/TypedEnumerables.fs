module private Cistern.Linq.FSharp.TypedEnumerables

open Cistern.Linq.ChainLinq.Optimizations

[<Struct; NoComparison; NoEquality>]
type ImmutableListEnumerator<'T> =
    val mutable state : list<'T>

    new (l:list<'T>) = { state = Unchecked.defaultof<'T> :: l }

    interface System.Collections.Generic.IEnumerator<'T> with
        member this.Current: 'T = match this.state with | hd :: _ -> hd | _ -> Unchecked.defaultof<_>

    interface System.IDisposable with
        member __.Dispose() = ()

    interface System.Collections.IEnumerator with
        member this.MoveNext () = match this.state with | [] | _ :: [] -> this.state <- []; false | _ :: tl -> this.state <- tl; true
        member __.Current  = raise (System.NotImplementedException())
        member __.Reset () = raise (System.NotImplementedException())

[<Struct; NoComparison; NoEquality>]
type ImmutableListEnumerable<'T>(lst:list<'T>) =
    interface ITypedEnumerable<'T, ImmutableListEnumerator<'T>> with
        member __.Source = upcast lst
        member __.TryGetSourceAsSpan _ = false
        member __.GetEnumerator () = new ImmutableListEnumerator<'T>(lst)
