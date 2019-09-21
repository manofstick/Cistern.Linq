namespace Cistern.Linq.FSharp.Consumers

open Cistern.Linq.ChainLinq
open System.Collections.Generic

type Pick<'T,'U>(chooser:'T->option<'U>) =
    inherit Consumer<'T, 'U>(Unchecked.defaultof<_>)

    let mutable found = false

    override __.ProcessNext input =
        match chooser input with
        | None -> ChainStatus.Flow
        | Some item ->
            found <- true
            base.Result <- item
            ChainStatus.Stop

    override __.ChainComplete _ =
        if not found then
            raise (KeyNotFoundException "An index satisfying the predicate was not found in the collection.")

        ChainStatus.Stop