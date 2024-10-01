﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bearded.Monads
{
    public static class Traversals
    {
        [DebuggerStepThrough]
        public static Try<IEnumerable<B>> Traverse<A, B>(
            this IEnumerable<A> enumerable,
            Func<A, Try<B>> callback)
        {
            IEnumerable<Try<B>> EnumerateCallback()
            {
                foreach(var enume in enumerable){
                    var execution = callback(enume);
                    yield return execution;
                    if (execution.IsError){
                        yield break;
                    }
                }
            }        

            var mapped = EnumerateCallback().ToList();
            var maybeError = mapped.FirstOrNone(x => x.IsError).Select(x => x.AsError().Value);

            return !maybeError.IsSome
                ? Try<IEnumerable<B>>.Create(mapped.Select(x => x.AsSuccess().Value))
                : maybeError.ForceValue();
        }

        [DebuggerStepThrough]
        public static IEnumerable<Try<B>> Traverse<A, B>(
            this Try<A> either,
            Func<A, IEnumerable<B>> callback
        )
        {
            var tryEnumerable = either.Select(callback);

            return tryEnumerable.IsSuccess
                ? tryEnumerable.AsSuccess().Value.Select(x => x.AsTry())
                : new[] {Try<B>.Create(tryEnumerable.AsError().Value)};
        }

        [DebuggerStepThrough]
        public static Option<IEnumerable<B>>
            Traverse<A, B>(this IEnumerable<A> enumerable, Func<A, Option<B>> callback) =>
            enumerable.Select(callback).AllOrNone();

        [DebuggerStepThrough]
        public static IEnumerable<Option<B>> Traverse<A, B>(this Option<A> option, Func<A, IEnumerable<B>> callback)
        {
            var optionEnumerable = option.Select(callback);

            return option.IsSome
                ? optionEnumerable.ForceValue().Select(x => x.AsOption())
                : Enumerable.Empty<Option<B>>();
        }

        [DebuggerStepThrough]
        public static Task<Option<B>> Traverse<A, B>(this Option<A> option, Func<A, Task<B>> callback)
        {
            var thing = option.Select(callback);

            return thing.IsSome
                ? thing.ForceValue().Select(x => x.AsOption())
                : Task.FromResult(Option<B>.None);
        }

        [DebuggerStepThrough]
        public static Task<Either<B, E>> Traverse<A, B, E>(this Either<A, E> either, Func<A, Task<B>> callback)
        {
            var thing = either.Select(callback);
 
            return thing.IsSuccess
                ? thing.AsSuccess.Value.Select(x => Either<B, E>.Create(x))
                : Task.FromResult(Either<B,E>.CreateError(either.AsError.Value));
        }
    }
}
