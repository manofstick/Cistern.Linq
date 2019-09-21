namespace Cistern.Linq.FSharp.Links

open Cistern.Linq.ChainLinq

type ChooseActivity<'T,'U>(chooser:'T->option<'U>, next) =
    inherit Cistern.Linq.ChainLinq.Activity<'T,'U>(next)

    override __.ProcessNext input =
        match chooser input with
        | None -> ChainStatus.Filter
        | Some item -> base.Next item

type Choose<'T,'U>(chooser:'T -> 'U option) =
    interface Cistern.Linq.ChainLinq.ILink<'T, 'U> with
        member __.Compose activity = 
            upcast ChooseActivity(chooser, activity)
