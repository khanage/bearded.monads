using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class OptionUnsafeUsafeExtensions
    {
        [DebuggerStepThrough]
        public static Option<A> NoneIfNull<A>(this OptionUnsafe<A> option) => option
            .Where(a => !ReferenceEquals(null, a))
            .Select(Option.Return)
            .Else(() => Option<A>.None);

        [DebuggerStepThrough]
        public static OptionUnsafe<A> AsOptionUnsafe<A>(this Option<A> option) => option
            .Select(OptionUnsafe.Return).Else(() => OptionUnsafe<A>.None);

        [DebuggerStepThrough]
        public static void Do<A>(this OptionUnsafe<A> option, Action<A> callback)
        {
            option.Do(callback, () => { });
        }

        [DebuggerStepThrough]
        public static A ElseDefault<A>(this OptionUnsafe<A> option)
        {
            return option.Else(() => default(A));
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<Tuple<A, B>> Concat<A, B>(this OptionUnsafe<A> oa, OptionUnsafe<B> ob)
        {
            return oa.SelectMany(a => ob.Map(b => Tuple.Create(a, b)));
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> Where<A>(this OptionUnsafe<A> option, Predicate<A> pred)
        {
            return option.SelectMany(a => pred(a) ? option : OptionUnsafe<A>.None);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> Empty<A>(this OptionUnsafe<A> option, Action nullCallback)
        {
            return option.WhenNone(nullCallback);
        }


        [DebuggerStepThrough]
        public static OptionUnsafe<A> WhenSome<A>(this OptionUnsafe<A> option, Action<A> callback)
        {
            option.Do(callback, () => { });

            return option;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> WhenNone<A>(this OptionUnsafe<A> option, Action callback)
        {
            option.Do(a => { }, callback);

            return option;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<B> SelectMany<A, B>(this OptionUnsafe<A> option, Func<A, OptionUnsafe<B>> mapper)
        {
            return option.Map(mapper).Else(() => OptionUnsafe<B>.None);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<C> SelectMany<A, B, C>(this OptionUnsafe<A> ma, Func<A, OptionUnsafe<B>> mapB, Func<A, B, C> mapper)
        {
            return ma.SelectMany(a => mapB(a).Map(b => mapper(a, b)));
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> Or<A>(this OptionUnsafe<A> left, OptionUnsafe<A> right)
        {
            return left | right;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> Or<A>(this OptionUnsafe<A> left, Func<OptionUnsafe<A>> right)
        {
            return left | right;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> FirstOrDefault<A>(this IEnumerable<OptionUnsafe<A>> options)
        {
            foreach (var option in (options ?? Enumerable.Empty<OptionUnsafe<A>>()).Where(option => option.IsSome))
            {
                return option;
            }

            return OptionUnsafe<A>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> AsOptionUnsafe<A>(this A value)
        {
            return value;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> Flatten<A>(this OptionUnsafe<OptionUnsafe<A>> option)
        {
            return option.SelectMany(x => x);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> ThenUnsafe<A>(this bool predicate, Func<A> callbackForTrue)
        {
            return predicate ? callbackForTrue() : OptionUnsafe<A>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<bool> NoneIfFalseUnsafe(this bool val)
        {
            return true.AsOptionUnsafe().Where(_ => val);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<Value> MaybeGetValueUnsafe<Key, Value>(this IDictionary<Key, Value> dict, Key key)
        {
            return dict.TryGetValue(key, out Value v) ? v : OptionUnsafe<Value>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<IEnumerable<Value>> MaybeGetValuesUnsafe<Key, Value>(this ILookup<Key, Value> lookup, Key key)
        {
            return lookup.Contains(key) ? lookup[key].AsOptionUnsafe() : OptionUnsafe<IEnumerable<Value>>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<Result> IfTrueThenUnsafe<Result>(this bool lookup, Result result)
        {
            return lookup.IfTrueThenUnsafe(() => result);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<Result> IfTrueThenUnsafe<Result>(this bool lookup, Func<Result> result)
        {
            return lookup ? result() : OptionUnsafe<Result>.None;
        }

        [DebuggerStepThrough]
        public static IEnumerable<A> ConcatOptionUnsafes<A>(this IEnumerable<OptionUnsafe<A>> options)
        {
            return options.SelectMany(MaybeAsEnumerable);
        }

        public static IEnumerable<A> MaybeAsEnumerable<A>(this OptionUnsafe<A> option)
        {
            return option.Map(a => Enumerable.Repeat(a, 1)).ElseEmpty();
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<CastTarget> MaybeCastUnsafe<CastTarget>(this Object current)
        {
            if (current is CastTarget) return (CastTarget) current;

            return OptionUnsafe<CastTarget>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<B> Select<A, B>(this OptionUnsafe<A> option, Func<A, B> f)
        {
            return option.Map(f);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> SingleOrNoneUnsafe<A>(this IEnumerable<A> items)
        {
            var ret = items.Take(2).ToList();

            return ret.Count != 1 ? OptionUnsafe<A>.None : ret[0];
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> SingleOrNoneUnsafe<A>(this IEnumerable<A> items, Func<A, bool> predicate)
        {
            return items.Where(predicate).SingleOrNoneUnsafe();
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<B> SingleOrNoneUnsafe<A, B>(this IEnumerable<A> items, Func<A, OptionUnsafe<B>> thing)
        {
            return items.Select(thing).ConcatOptionUnsafes().SingleOrNoneUnsafe();
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> FirstOrNoneUnsafe<A>(this IEnumerable<A> items)
        {
            return items.FirstOrNoneUnsafe(_ => true);
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> FirstOrNoneUnsafe<A>(this IEnumerable<A> items, Func<A, bool> predicate)
        {
            foreach (var item in items.Where(predicate))
                return item;

            return OptionUnsafe<A>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<B> FirstOrNoneUnsafe<A, B>(this IEnumerable<A> items, Func<A, OptionUnsafe<B>> thing)
        {
            return items.Select(thing).FirstOrNoneUnsafe(o => o.IsSome).Flatten();
        }

        [DebuggerStepThrough]
        public static A ElseThrow<A>(this OptionUnsafe<A> option, Func<Exception> exceptionCallback)
        {
            return option.Else(() => throw exceptionCallback());
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<A> MaybeGetReference<A>(this WeakReference<A> refToItem) where A : class
        {
            return refToItem.TryGetTarget(out var item) ? item : OptionUnsafe<A>.None;
        }

        [DebuggerStepThrough]
        public static IEnumerable<A> ElseEmpty<A>(this OptionUnsafe<IEnumerable<A>> items)
        {
            return items.Else(Enumerable.Empty<A>);
        }

        [DebuggerStepThrough]
        public static A ElseNew<A>(this OptionUnsafe<A> items) where A : new()
        {
            return items.Else(() => new A());
        }

        [DebuggerStepThrough]
        public static async Task DoAsync<A>(this OptionUnsafe<A> source, Func<A, Task> act)
        {
            if (source == null || !source.IsSome) return;

            await act(source.ForceValue());
        }

        [DebuggerStepThrough]
        public static async Task<OptionUnsafe<B>> MapAsync<A, B>(this OptionUnsafe<A> source, Func<A, Task<B>> act)
        {
            if (source == null || !source.IsSome) return OptionUnsafe<B>.None;

            return await act(source.ForceValue());
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<IEnumerable<A>> AllOrNone<A>(this IEnumerable<OptionUnsafe<A>> options)
        {
            var result = new List<A>();
            return options.All(option => option.WhenSome(result.Add))
                ? result.AsOptionUnsafe<IEnumerable<A>>()
                : OptionUnsafe<IEnumerable<A>>.None;
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<IEnumerable<B>> Traverse<A, B>(this IEnumerable<A> enumerable, Func<A, OptionUnsafe<B>> callback)
        {
            return enumerable.Select(callback).AllOrNone();
        }

        [DebuggerStepThrough]
        public static OptionUnsafe<IEnumerable<A>> Sequence<A>(this IEnumerable<OptionUnsafe<A>> incoming)
            => incoming.Traverse(id);
    }
}
