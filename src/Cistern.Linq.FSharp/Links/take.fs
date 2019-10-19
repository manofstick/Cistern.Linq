namespace Cistern.Linq.FSharp.Links

open Cistern.Linq

type TakeActivity<'T>(count:int, next) =
    inherit Cistern.Linq.Activity<'T,'T>(next)

    let mutable index = 0

    override __.ProcessNext input =
        if index >= count then
            ChainStatus.Stop
        else
            index <- Checked.(+) index 1
            if index >= count then
                ChainStatus.Stop ||| base.Next input
            else
                base.Next input

    override __.ChainComplete status =
        if not (status.HasFlag ChainStatus.Stop) && index < count then
            raise (System.InvalidOperationException (sprintf "tried to take The input sequence has an insufficient number of elements. %d past the end of the seq" (count-index)))

        base.ChainComplete status

type Take<'T>(count:int) =
    interface Cistern.Linq.ILink<'T, 'T> with
        member __.Compose activity = 
            upcast TakeActivity(count, activity)
