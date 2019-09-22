namespace Cistern.Linq.FSharp.Links

open Cistern.Linq.ChainLinq
open System
open System.Runtime.CompilerServices

[<Sealed>]
type ChooseVActivity<'T,'U>(chooser:'T->ValueOption<'U>, next) =
    inherit Cistern.Linq.ChainLinq.Activity<'T,'U>(next)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member __.Process input =
        match chooser input with
        | ValueNone -> ChainStatus.Filter
        | ValueSome item -> base.Next item

    override this.ProcessNext input = this.Process input

(*
    interface Optimizations.IHeadStart<'T> with
        member this.Execute<'Enumerable, 'Enumerator 
                                when 'Enumerable :> Optimizations.ITypedEnumerable<'T, 'Enumerator> 
                                and 'Enumerator :> System.Collections.Generic.IEnumerator<'T>
                            >(source:'Enumerable) : ChainStatus = 
            use mutable e = source.GetEnumerator ()
            while e.MoveNext () do
                this.Process e.Current |> ignore
            ChainStatus.Flow

        member this.Execute (source:ReadOnlySpan<'T>) : ChainStatus =
            for input in source do
                this.Process input |> ignore
            ChainStatus.Flow
*)

type ChooseV<'T,'U>(chooser:'T -> ValueOption<'U>) =
    interface Cistern.Linq.ChainLinq.ILink<'T, 'U> with
        member __.Compose activity = 
            upcast ChooseVActivity(chooser, activity)
