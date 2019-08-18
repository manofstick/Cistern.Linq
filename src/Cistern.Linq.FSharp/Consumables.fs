namespace Cistern.Linq.FSharp.Consumables

open Cistern.Linq.ChainLinq
open Cistern.Linq.ChainLinq.Consumables

type ResultConsumer<'T>(init) =
    inherit Consumer<'T, 'T>(init)

    override this.ProcessNext item =
        this.Result <- item
        ChainStatus.Flow

[<Sealed>]
type Unfold<'State, 'T, 'V>(f:'State->option<'T*'State>, seed:'State, link:Link<'T, 'V>) =
    inherit Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<'V, 'T>(link)

    override __.Create    (first:Link<'T, 'V>) = Unfold<'State, 'T, 'V>(f, seed, first) :> Consumable<'V>
    override __.Create<'W>(first:Link<'T, 'W>) = Unfold<'State, 'T, 'W>(f, seed, first) :> Consumable<'W>

    override __.GetEnumerator () =
        let enumerable = 
            let tail = ResultConsumer Unchecked.defaultof<'V>
            let path = link.Compose tail
            seq {
                let mutable state = seed
                let mutable running = true
                while running do
                    match f state with
                    | None -> running <- false
                    | Some (item, next) ->
                        state <- next
                        match path.ProcessNext item with
                        | ChainStatus.Flow -> yield tail.Result
                        | ChainStatus.Filter -> ()
                        | _ -> running <- false
            }
        enumerable.GetEnumerator ()
    
    override __.Consume(consumer:Consumer<'V> ) =
        let chain = link.Compose consumer
        try
            let rec iterate state =
                match f state with
                | None -> ()
                | Some (item, next) ->
                    let state = chain.ProcessNext item
                    if not (ProcessNextResultHelper.IsStopped state) then
                        iterate next

            iterate seed

            chain.ChainComplete();
        finally
            chain.ChainDispose();


