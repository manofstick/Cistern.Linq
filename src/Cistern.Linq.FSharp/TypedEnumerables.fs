module private Cistern.Linq.FSharp.TypedEnumerables

open Cistern.Linq.ChainLinq.Optimizations

[<Struct; NoComparison; NoEquality>]
type FSharpListEnumerator<'T> =
    val mutable state : list<'T>
    val mutable current : 'T

    new (l:list<'T>) = {
        state = l
        current = Unchecked.defaultof<'T>
    }

    interface System.Collections.Generic.IEnumerator<'T> with
        member this.Current: 'T = this.current

    interface System.IDisposable with
        member __.Dispose() = ()

    interface System.Collections.IEnumerator with
        member this.MoveNext () = 
            match this.state with
            | [] -> false
            | hd :: tl ->
                this.current <- hd
                this.state <- tl
                true

        member __.Current  = raise (System.NotImplementedException())
        member __.Reset () = raise (System.NotImplementedException())

[<Struct; NoComparison; NoEquality>]
type FSharpListEnumerable<'T>(lst:list<'T>) =
    interface ITypedEnumerable<'T, FSharpListEnumerable<'T>, FSharpListEnumerator<'T>> with
        member __.Source = upcast lst
        member __.TryGetSourceAsSpan _ = false
        member __.TryLength = System.Nullable lst.Length // is this O(1)??
        member __.GetEnumerator () = new FSharpListEnumerator<'T>(lst)
        member __.TrySkip (count, enumerable:byref<_>) =
            let rec skip count = function
            | [] -> FSharpListEnumerable []
            | lst when count = 0 -> FSharpListEnumerable lst
            | _ :: tl -> skip (count-1) tl
            enumerable <- skip count lst
            true

