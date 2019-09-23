using Cistern.Linq.ChainLinq;
using Microsoft.FSharp.Core;
using System;

namespace Cistern.Linq.FSharp.Links
{
    /*
     * From f#, but due to https://github.com/dotnet/fsharp/issues/7607 has been moved to c#
     * 
        namespace Cistern.Linq.FSharp.Links

        open Cistern.Linq.ChainLinq
        open System.Runtime.CompilerServices

        [<Sealed>]
        type ChooseVActivity<'T,'U>(chooser:'T->ValueOption<'U>, next) =
            inherit Cistern.Linq.ChainLinq.Activity<'T,'U>(next)

            [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
            member __.Process input =
                match chooser input with
                | ValueNone -> ChainStatus.Filter
                | ValueSome item -> base.Next item

            override this.ProcessNext input = this.Process input

        type ChooseV<'T,'U>(chooser:'T -> ValueOption<'U>) =
            interface Cistern.Linq.ChainLinq.ILink<'T, 'U> with
                member __.Compose activity = 
                    upcast ChooseVActivity(chooser, activity)

    */

    internal sealed class ChooseVActivity<T, U>
        : Activity<T, U>
        , ChainLinq.Optimizations.IHeadStart<T>
    {
        private readonly FSharpFunc<T, FSharpValueOption<U>> chooser;

        public ChooseVActivity(FSharpFunc<T, FSharpValueOption<U>> chooser, Chain<U> next) : base(next)
        {
            this.chooser = chooser;
        }

        ChainStatus ChainLinq.Optimizations.IHeadStart<T>.Execute(ReadOnlySpan<T> source)
        {
            var status = ChainStatus.Flow;
            foreach(var input in source)
            {
                var maybe = chooser.Invoke(input);
                if (maybe.IsSome)
                {
                    status = Next(maybe.Item);
                    if (status.IsStopped())
                        break;
                }
            }
            return status;
        }

        ChainStatus ChainLinq.Optimizations.IHeadStart<T>.Execute<Enumerable, Enumerator>(Enumerable source)
        {
            var status = ChainStatus.Flow;
            foreach (var input in source)
            {
                var maybe = chooser.Invoke(input);
                if (maybe.IsSome)
                {
                    status = Next(maybe.Item);
                    if (status.IsStopped())
                        break;
                }
            }
            return status;
        }

        public override ChainStatus ProcessNext(T input)
        {
            var maybe = chooser.Invoke(input);
            return maybe.IsSome ? Next(maybe.Item) : ChainStatus.Filter;
        }
    }

    public class ChooseV<T, U>
        : ILink<T, U>
    {
        private readonly FSharpFunc<T, FSharpValueOption<U>> chooser;

        public ChooseV(FSharpFunc<T, FSharpValueOption<U>> chooser) => this.chooser = chooser;

        Chain<T> ILink<T, U>.Compose(Chain<U> activity) => new ChooseVActivity<T, U>(chooser, activity);
    }
}