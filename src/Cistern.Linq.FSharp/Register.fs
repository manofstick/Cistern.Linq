module Cistern.Linq.FSharp.Register

open Cistern.Linq.ChainLinq
open System.Collections.Generic

type private TryFindImmutableTypes () =
    interface Utils.ITryFindSpecificType with
        member __.Namespace = "Microsoft.FSharp.Collections"

        member __.TryCreateSpecific<'T, 'U, 'Construct when 'Construct :> Utils.IConstruct<'T, 'U>> (construct:'Construct, e:IEnumerable<'T>, name:string) =
            if name.Length <= 6 then null
            else
                let sixthChar = name.[6] //  here  |
                                         //       \|/
                                         // 'FSharpXXXX'
                                         //  0123456789
                if sixthChar = 'L' then
                    match e with
                    | :? list<'T> as l -> construct.Create (TypedEnumerables.FSharpListEnumerable<'T> l)
                    | _ -> null
                else 
                    null

module private TryFindImmutableTypesInstance =
    let Instance = TryFindImmutableTypes () :> Utils.ITryFindSpecificType

let RegisterFSharpCollections () =
    Utils.Register TryFindImmutableTypesInstance.Instance