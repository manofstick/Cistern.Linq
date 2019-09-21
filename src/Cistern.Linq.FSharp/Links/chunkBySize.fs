namespace Cistern.Linq.FSharp.Links

open Cistern.Linq.ChainLinq

type ChunkBySizeActivity<'T>(chunkBySize, next) =
    inherit Cistern.Linq.ChainLinq.Activity<'T,array<'T>>(next)

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
            Cistern.Linq.ChainLinq.ChainStatus.Filter
            
    override __.ChainComplete status =
        if ProcessNextResultHelper.IsStopped status || isNull current then
            base.ChainComplete status
        else
            System.Array.Resize (&current, idx)
            base.Next current |> ignore
            current <- Unchecked.defaultof<_>
            base.ChainComplete ChainStatus.Flow

type ChunkBySize<'T>(chunkBySize:int) =
    interface Cistern.Linq.ChainLinq.ILink<'T, array<'T>> with
        member __.Compose activity = 
            upcast ChunkBySizeActivity(chunkBySize, activity)
