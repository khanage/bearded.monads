using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if NET45
using System.Threading.Tasks;
#endif

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

            var a = ma.ForceValue();

            var mb = mapB(a);

            if (!mb.IsSome)
            {
                return defaultValue;
            }

            var b = mb.ForceValue();

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
            return val ? (Option<bool>) true : Option<bool>.None;
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

        [DebuggerStepThrough]
        public static Option<IEnumerable<Value>> MaybeGetValues<Key, Value>(this ILookup<Key, Value> lookup, Key key)
        {
            if (lookup.Contains(key))
            {
                return lookup[key].AsOption();
            }

            return Option<IEnumerable<Value>>.None;
        }

        [DebuggerStepThrough]
        public static Option<Result> IfTrueThen<Result>(this bool lookup, Result result)
        {
            return lookup.IfTrueThen(() => result);
        }

        [DebuggerStepThrough]
        public static Option<Result> IfTrueThen<Result>(this bool lookup, Func<Result> result)
        {
            if (lookup) return result();
            return Option<Result>.None;
        }

        [DebuggerStepThrough]
        public static IEnumerable<A> ConcatOptions<A>(this IEnumerable<Option<A>> options)
        {
            return options.Where(o => o.IsSome).Select(o => o.ForceValue());
        }

        [DebuggerStepThrough]
        public static Option<CastTarget> MaybeCast<CastTarget>(this Object current)
        {
            if (current is CastTarget) return (CastTarget) current;

            return Option<CastTarget>.None;
        }

        [DebuggerStepThrough]
        public static Option<B> Select<A, B>(this Option<A> option, Func<A, B> f)
        {
            return option.Map(f);
        }

        [DebuggerStepThrough]
        public static Option<A> SingleOrNone<A>(this IEnumerable<A> items)
        {
            var ret = items.Take(2).ToList();

            if (ret.Count != 1)
                return Option<A>.None;

            return ret[0];
        }

        [DebuggerStepThrough]
        public static Option<A> SingleOrNone<A>(this IEnumerable<A> items, Func<A, bool> predicate)
        {
            return items.Where(predicate).SingleOrNone();
        }

        [DebuggerStepThrough]
        public static Option<B> SingleOrNone<A, B>(this IEnumerable<A> items, Func<A, Option<B>> thing)
        {
            return items.Select(thing).ConcatOptions().SingleOrNone();
        }

        [DebuggerStepThrough]
        public static Option<A> FirstOrNone<A>(this IEnumerable<A> items)
        {
            return items.FirstOrNone(_ => true);
        }

        [DebuggerStepThrough]
        public static Option<A> FirstOrNone<A>(this IEnumerable<A> items, Func<A, bool> predicate)
        {
            var firstItems = items.Where(predicate).Take(1).ToList();

            if (firstItems.Count == 0)
                return Option<A>.None;

            return firstItems[0];
        }

        [DebuggerStepThrough]
        public static Option<B> FirstOrNone<A, B>(this IEnumerable<A> items, Func<A, Option<B>> thing)
        {
            return items.Select(thing).FirstOrNone(o => o.IsSome).Flatten();
        }

        [DebuggerStepThrough]
        public static Option<T> AggregateOrNone<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return Option<T>.None;

                var result = e.Current;
                while (e.MoveNext())
                    result = func(result, e.Current);
                return result;
            }
        }

        [DebuggerStepThrough]
        public static Option<B> AggregateOrNone<A,B>(this IEnumerable<A> source, B seed, Func<B, A, B> func)
        {
            using (var e = source.GetEnumerator())
            {
                var result = seed;
                if (!e.MoveNext())
                    return Option<B>.None;
                do
                {
                    result = func(result, e.Current);
                } while (e.MoveNext());

                return result;
            }
        }
        [DebuggerStepThrough]
        public static Option<B> AggregateOrNone<A,B>(this IEnumerable<Option<A>> source, Option<B> seed, Func<Option<B>, Option<A>, Option<B>> func)
        {
            using (var e = source.GetEnumerator())
            {
                var result = seed;
                if (!e.MoveNext())
                    return Option<B>.None;
                do
                {
                    result = func(result, e.Current);
                } while (e.MoveNext());

                return result;
            }
        }

        [DebuggerStepThrough]
        public static Option<T> AggregateOrNone<T>(this IEnumerable<Option<T>> source,
            Func<Option<T>, Option<T>, Option<T>> func)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return Option<T>.None;

                var result = e.Current;
                while (e.MoveNext())
                    result = func(result, e.Current);
                return result;
            }
        }

        [DebuggerStepThrough]
        public static A ElseThrow<A>(this Option<A> option, Func<Exception> exceptionCallback)
        {
            if (option.IsSome) return option.ForceValue();
            throw exceptionCallback();
        }

#if NET45
        [DebuggerStepThrough]
        public static Option<A> MaybeGetReference<A>(this WeakReference<A> refToItem) where A : class
        {
            A item;
            if (refToItem.TryGetTarget(out item))
                return item;

            return Option<A>.None;
        }
#endif

        [DebuggerStepThrough]
        public static IEnumerable<A> ElseEmpty<A>(this Option<IEnumerable<A>> items)
        {
            return items.Else(Enumerable.Empty<A>);
        }

#if NET45
        [DebuggerStepThrough]
        public static async Task DoAsync<A>(this Option<A> source, Func<A, Task> act)
        {
            if (source == null || !source.IsSome) return;

            await act(source.ForceValue());
        }

        [DebuggerStepThrough]
        public static async Task<Option<B>> MapAsync<A, B>(this Option<A> source, Func<A, Task<B>> act)
        {
            if (source == null || !source.IsSome) return Option<B>.None;

            return await act(source.ForceValue());
        }
#endif
    }
}
