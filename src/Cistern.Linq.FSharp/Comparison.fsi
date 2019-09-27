module Cistern.Linq.FSharp.Comparison

// This is a subset of the file
// https://github.com/manofstick/visualfsharp/blob/notagged/src/fsharp/FSharp.Core/prim-types.fs
// which was modified in the PR https://github.com/dotnet/fsharp/pull/5112
// that was never merged to the main line.

val FastGenericEqualityComparerFromTable<'T> : System.Collections.Generic.IEqualityComparer<'T>