namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type InitEnumerator<'State, 'T> =
    val count           : int
    val initializer     : int->'T
    val mutable index   : int
    val mutable current : 'T

    new (count:int, initializer:int->'T) = {
         count = count-1
         initializer = initializer
         index = -1
         current = Unchecked.defaultof<_>
    }

    interface IEnumerator<'T> with
        member this.Current : 'T = this.current
        member this.Current : obj = box this.current
        member this.MoveNext (): bool = 
            if this.index >= this.count then false
            else
                this.index <- this.index + 1
                this.current <- this.initializer this.index
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


