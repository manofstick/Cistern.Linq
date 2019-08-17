namespace Cistern.Linq.FSharp

open Cistern.Linq
open System

type Linq =
    static member inline map (f:'a->'b) (e:seq<'a>)           = Enumerable.Select (e, f)
    static member inline mapi (f:int->'a->'b) (e:seq<'a>)     = Enumerable.Select (e, fun a idx -> f idx a)
                                                              
    static member inline filter (f:'a->bool) (e:seq<'a>)      = Enumerable.Where (e, f)
                                                              
    static member inline reduce (f:'a->'a->'a) (e:seq<'a>)    = Enumerable.Aggregate (e, fun a c -> f a c)

    static member inline fold (f:'s->'a->'s) seed (e:seq<'a>) = Enumerable.Aggregate (e, seed, fun a c -> f a c)

    static member inline sum (e:seq<float>)                   = Enumerable.Sum e
    static member inline sum (e:seq<float32>)                 = Enumerable.Sum e
    static member inline sum (e:seq<decimal>)                 = Enumerable.Sum e
    static member inline sum (e:seq<int>)                     = Enumerable.Sum e
    static member inline sum (e:seq<int64>)                   = Enumerable.Sum e
    static member inline sum (e:seq<Nullable<float>>)         = Enumerable.Sum e
    static member inline sum (e:seq<Nullable<float32>>)       = Enumerable.Sum e
    static member inline sum (e:seq<Nullable<decimal>>)       = Enumerable.Sum e
    static member inline sum (e:seq<Nullable<int>>)           = Enumerable.Sum e
    static member inline sum (e:seq<Nullable<int64>>)         = Enumerable.Sum e
                                                              
    static member inline min (e:seq<float>)                   = Enumerable.Min e
    static member inline min (e:seq<float32>)                 = Enumerable.Min e
    static member inline min (e:seq<decimal>)                 = Enumerable.Min e
    static member inline min (e:seq<int>)                     = Enumerable.Min e
    static member inline min (e:seq<int64>)                   = Enumerable.Min e
    static member inline min (e:seq<Nullable<float>>)         = Enumerable.Min e
    static member inline min (e:seq<Nullable<float32>>)       = Enumerable.Min e
    static member inline min (e:seq<Nullable<decimal>>)       = Enumerable.Min e
    static member inline min (e:seq<Nullable<int>>)           = Enumerable.Min e
    static member inline min (e:seq<Nullable<int64>>)         = Enumerable.Min e
                                                              
    static member inline max (e:seq<float>)                   = Enumerable.Max e
    static member inline max (e:seq<float32>)                 = Enumerable.Max e
    static member inline max (e:seq<decimal>)                 = Enumerable.Max e
    static member inline max (e:seq<int>)                     = Enumerable.Max e
    static member inline max (e:seq<int64>)                   = Enumerable.Max e
    static member inline max (e:seq<Nullable<float>>)         = Enumerable.Max e
    static member inline max (e:seq<Nullable<float32>>)       = Enumerable.Max e
    static member inline max (e:seq<Nullable<decimal>>)       = Enumerable.Max e
    static member inline max (e:seq<Nullable<int>>)           = Enumerable.Max e
    static member inline max (e:seq<Nullable<int64>>)         = Enumerable.Max e


