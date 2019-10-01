namespace Cistern.Linq.FSharp.Links

open Cistern.Linq

type ChunkBySizeActivity<'T>(chunkBySize, next) =
    inherit Cistern.Linq.Activity<'T,array<'T>>(next)

    let mutable current = Unchecked.defaultof<_>
    let mutable idx = Unchecked.defaultof<_>

    override __.ProcessNext input =
        if isNull current then
            current <- Array.zeroCreate chunkBySize
            idx <- 0

        current.[idx] <- input
        idx <- idx + 1

        if idx = current.Length then
            let output = current
            current <- Unchecked.defaultof<_>
            base.Next output
        else
            Cistern.Linq.ChainStatus.Filter
            
    override __.ChainComplete status =
        if ProcessNextResultHelper.IsStopped status || isNull current then
            base.ChainComplete status
        else
            System.Array.Resize (&current, idx)
            base.Next current |> ignore
            current <- Unchecked.defaultof<_>
            base.ChainComplete ChainStatus.Flow

type ChunkBySize<'T>(chunkBySize:int) =
    interface Cistern.Linq.ILink<'T, array<'T>> with
        member __.Compose activity = 
            upcast ChunkBySizeActivity(chunkBySize, activity)

    interface Cistern.Linq.Optimizations.ILinkFastCount with
        member __.SupportedAsConsumer = true
        member __.FastCountAdjustment count =
            let count = 
                if count = 0
                then 0
                else ((count-1) / chunkBySize) + 1
            System.Nullable count