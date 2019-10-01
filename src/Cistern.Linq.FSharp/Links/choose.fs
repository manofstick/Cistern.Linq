namespace Cistern.Linq.FSharp.Links

open Cistern.Linq

type ChooseActivity<'T,'U>(chooser:'T->option<'U>, next) =
    inherit Cistern.Linq.Activity<'T,'U>(next)

    override __.ProcessNext input =
        match chooser input with
        | None -> ChainStatus.Filter
        | Some item -> base.Next item

type Choose<'T,'U>(chooser:'T->option<'U>) =
    interface Cistern.Linq.ILink<'T, 'U> with
        member __.Compose activity = 
            upcast ChooseActivity(chooser, activity)
