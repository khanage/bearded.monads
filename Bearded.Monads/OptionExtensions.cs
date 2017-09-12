using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class OptionExtensions
    {
        [DebuggerStepThrough]
        public static void Do<A>(this Option<A> option, Action<A> callback)
        {
            option.Do(callback, () => { });
        }

        [DebuggerStepThrough]
        public static A ElseDefault<A>(this Option<A> option)
        {
            return option.Else(() => default(A));
        }

        [DebuggerStepThrough]
        public static Option<Tuple<A, B>> Concat<A, B>(this Option<A> oa, Option<B> ob)
        {
            return oa.SelectMany(a => ob.Map(b => Tuple.Create(a, b)));
        }

        [DebuggerStepThrough]
        public static Option<A> Where<A>(this Option<A> option, Predicate<A> pred)
        {
            return option.SelectMany(a => pred(a) ? option : Option<A>.None);
        }

        [DebuggerStepThrough]
        public static Option<A> Empty<A>(this Option<A> option, Action nullCallback)
        {
            return option.WhenNone(nullCallback);
        }


        [DebuggerStepThrough]
        public static Option<A> WhenSome<A>(this Option<A> option, Action<A> callback)
        {
            option.Do(callback, () => { });

            return option;
        }

        [DebuggerStepThrough]
        public static Option<A> WhenNone<A>(this Option<A> option, Action callback)
        {
            option.Do(a => { }, callback);

            return option;
        }

        [DebuggerStepThrough]
        public static Option<B> SelectMany<A, B>(this Option<A> option, Func<A, Option<B>> mapper)
        {
            return option.Map(mapper).Else(() => Option<B>.None);
        }

        [DebuggerStepThrough]
        public static Option<C> SelectMany<A, B, C>(this Option<A> ma, Func<A, Option<B>> mapB, Func<A, B, C> mapper)
        {
            return ma.SelectMany(a => mapB(a).Map(b => mapper(a, b)));
        }

        [DebuggerStepThrough]
        public static Option<A> Or<A>(this Option<A> left, Option<A> right)
        {
            return left | right;
        }

        [DebuggerStepThrough]
        public static Option<A> Or<A>(this Option<A> left, Func<Option<A>> right)
        {
            return left | right;
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
            return predicate ? callbackForTrue() : Option<A>.None;
        }

        [Obsolete("This call is equivalent to .AsOption")]
        [DebuggerStepThrough]
        public static Option<A> NoneIfNull<A>(this A val)
            where A : class
        {
            return val;
        }

        [DebuggerStepThrough]
        public static Option<bool> NoneIfFalse(this bool val)
        {
            return true.AsOption().Where(_ => val);
        }

        [DebuggerStepThrough]
        public static Option<A> NoneIfEmpty<A>(this A? nullable) where A : struct
        {
            return nullable ?? Option<A>.None;
        }

        [DebuggerStepThrough]
        public static Option<Value> MaybeGetValue<Key, Value>(this IDictionary<Key, Value> dict, Key key)
        {
            return dict.TryGetValue(key, out Value v) ? v : Option<Value>.None;
        }

        [DebuggerStepThrough]
        public static Option<IEnumerable<Value>> MaybeGetValues<Key, Value>(this ILookup<Key, Value> lookup, Key key)
        {
            return lookup.Contains(key) ? lookup[key].AsOption() : Option<IEnumerable<Value>>.None;
        }

        [DebuggerStepThrough]
        public static Option<Result> IfTrueThen<Result>(this bool lookup, Result result)
        {
            return lookup.IfTrueThen(() => result);
        }

        [DebuggerStepThrough]
        public static Option<Result> IfTrueThen<Result>(this bool lookup, Func<Result> result)
        {
            return lookup ? result() : Option<Result>.None;
        }

        [DebuggerStepThrough]
        public static IEnumerable<A> ConcatOptions<A>(this IEnumerable<Option<A>> options)
        {
            return options.SelectMany(MaybeAsEnumerable);
        }

        public static IEnumerable<A> MaybeAsEnumerable<A>(this Option<A> option)
        {
            return option.Map(a => Enumerable.Repeat(a, 1)).ElseEmpty();
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

            return ret.Count != 1 ? Option<A>.None : ret[0];
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
            foreach (var item in items.Where(predicate))
                return item;

            return Option<A>.None;
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
            return option.Else(() => throw exceptionCallback());
        }

        [DebuggerStepThrough]
        public static Option<A> MaybeGetReference<A>(this WeakReference<A> refToItem) where A : class
        {
            return refToItem.TryGetTarget(out var item) ? item : Option<A>.None;
        }

        [DebuggerStepThrough]
        public static IEnumerable<A> ElseEmpty<A>(this Option<IEnumerable<A>> items)
        {
            return items.Else(Enumerable.Empty<A>);
        }

        [DebuggerStepThrough]
        public static A ElseNew<A>(this Option<A> items) where A : new()
        {
            return items.Else(() => new A());
        }

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

        [DebuggerStepThrough]
        public static Option<IEnumerable<A>> AllOrNone<A>(this IEnumerable<Option<A>> options)
        {
            var result = new List<A>();
            return options.All(option => option.WhenSome(result.Add))
                ? result.AsOption<IEnumerable<A>>()
                : Option<IEnumerable<A>>.None;
        }

        [DebuggerStepThrough]
        public static Option<IEnumerable<A>> Sequence<A>(this IEnumerable<Option<A>> incoming)
            => incoming.Traverse(id);
    }
}
