namespace Cistern.Linq.FSharp.Links

open Cistern.Linq.ChainLinq
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

type ChooseV<'T,'U>(chooser:'T -> ValueOption<'U>) =
    interface Cistern.Linq.ChainLinq.ILink<'T, 'U> with
        member __.Compose activity = 
            upcast ChooseVActivity(chooser, activity)
