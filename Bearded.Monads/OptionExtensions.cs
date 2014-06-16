using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bearded.Monads
{
    public static class OptionExtensions
    {
        [DebuggerStepThrough]
        public static Option<C> SelectMany<A, B, C>(this Option<A> ma, Func<A, Option<B>> mapB, Func<A, B, C> mapper)
        {
            var defaultValue = Option<C>.None;

            if (!ma.IsSome)
            {
                return defaultValue;
            }

            var a = ma.Value;

            var mb = mapB(a);

            if (!mb.IsSome)
            {
                return defaultValue;
            }

            var b = mb.Value;

            return mapper(a, b);
        }

        [DebuggerStepThrough]
        public static Option<A> FirstOrDefault<A>(this IEnumerable<Option<A>> options)
        {
            foreach (var option in (options ?? Enumerable.Empty<Option<A>>()).Where(option => option.IsSome))
            {
                return option;
            }

            return Option<A>.None;
        }

        [DebuggerStepThrough]
        public static Option<A> AsOption<A>(this A value)
        {
            return value;
        }

        [DebuggerStepThrough]
        public static Option<A> Flatten<A>(this Option<Option<A>> option)
        {
            return option.SelectMany(x => x);
        }

        [DebuggerStepThrough]
        public static Option<A> Then<A>(this bool predicate, Func<A> callbackForTrue)
        {
            if (!predicate)
            {
                return Option<A>.None;
            }

            return callbackForTrue();
        }

        [DebuggerStepThrough]
        public static Option<A> NoneIfNull<A>(this A val)
            where A : class
        {
            if (val != null)
            {
                return val;
            }

            return Option<A>.None;
        }

        [DebuggerStepThrough]
        public static Option<bool> NoneIfFalse(this bool val)
        {
            return val ? true : Option<bool>.None;
        }

        [DebuggerStepThrough]
        public static Option<A> NoneIfEmpty<A>(this A? nullable) where A : struct 
        {
            if (nullable.HasValue)
                return nullable.Value;

            return Option<A>.None;
        }

        [DebuggerStepThrough]
        public static Option<Value> MaybeGetValue<Key, Value>(this IDictionary<Key, Value> dict, Key key)
        {
            Value v;

            if (!dict.TryGetValue(key, out v))
                return Option<Value>.None;

            return v;
        }
    }
}