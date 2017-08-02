using System;
using System.Collections.Generic;
using System.Linq;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class TryExtensions
    {
        public static Try<C> SelectMany<A, B, C>(this Try<A> ta, Func<A, Try<B>> map, Func<A, B, C> selector)
        {
            try
            {
                if (ta.IsError) return ta.AsError().Value;

                var a = ta.AsSuccess().Value;

                var tb = map(a);

                if (tb.IsError) return tb.AsError().Value;

                var b = tb.AsSuccess().Value;

                return selector(a, b);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Try<B> SelectMany<A, B>(this Try<A> ta, Func<A, Try<B>> map)
        {
            try
            {
                if (ta.IsError) return ta.AsError().Value;

                var a = ta.AsSuccess().Value;

                return map(a);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Option<A> AsOption<A>(this Try<A> either, Action<Exception> callback)
            => either.Else(Option<A>.Return, e =>
            {
                callback(e);
                return Option<A>.None;
            });

        public static EitherSuccessOrError<A, Exception> AsEither<A>(this Try<A> either)
            => either.AsEither(id);

        public static EitherSuccessOrError<A, Error> AsEither<A, Error>(this Try<A> either, Func<Exception, Error> errorMap)
            => either.Else(EitherSuccessOrError<A, Error>.Create, ex => errorMap(ex));

        public static Try<B> Select<A, B>(this Try<A> either, Func<A, B> projector)
        {
            return either.Map(projector);
        }

        public static Try<A> Where<A>(this Try<A> either,
            Predicate<A> predicate, Func<Exception> errorCallback)
        {
            if (either.IsError) return either;
            if (predicate(either.AsSuccess().Value)) return either;
            return errorCallback();
        }

        public static Result Else<A, Result>(this Try<A> either,
            Func<A, Result> happy,
            Func<Exception, Result> sad)
        {
            return either.IsSuccess
                ? happy(either.AsSuccess().Value)
                : sad(either.AsError().Value);
        }

        public static Try<Result> SafeCallback<Incoming, Result>(this Incoming item,
            Func<Incoming, Result> callback)
        {
            try
            {
                return callback(item);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Result Unify<Success, Result>(this Try<Success> either,
            Func<Success, Result> successFunc, Func<Exception, Result> errorFunc)
        {
            if (either.IsSuccess) return successFunc(either.AsSuccess().Value);
            return errorFunc(either.AsError().Value);
        }

        public static Success Else<Success>(this Try<Success> either,
            Func<Exception, Success> callback)
        {
            if (either.IsError) return callback(either.AsError().Value);

            return either.AsSuccess().Value;
        }

        public static Try<A> WhenSuccess<A>(this Try<A> either,
            Action<A> callbackForSuccess)
        {
            if (either.IsSuccess)
            {
                callbackForSuccess(either.AsSuccess().Value);
            }

            return either;
        }

        public static Try<A> WhenError<A>(this Try<A> either,
            Action<Exception> callbackForError)
        {
            if (either.IsError)
            {
                callbackForError(either.AsError().Value);
            }

            return either;
        }

        public static A ElseThrow<A>(this Try<A> either)
        {
            return either.Else(exc => { throw exc; });
        }

        public static Try<A> Flatten<A>(
            this Try<Try<A>> ee)
            => ee.SelectMany(Syntax.id);

        public static Try<IEnumerable<B>> Traverse<A, B>(
            this IEnumerable<A> enumerable,
            Func<A, Try<B>> callback)
        {
            var mapped = enumerable.Select(callback).ToList();

            var maybeError = mapped.FirstOrNone(x => x.IsError).Select(x => x.AsError().Value);

            return !maybeError.IsSome
                ? Try<IEnumerable<B>>.Create(mapped.Select(x => x.AsSuccess().Value))
                : maybeError.ForceValue();
        }

        public static Try<IEnumerable<A>> Sequence<A>(
            this IEnumerable<Try<A>> incoming)
            => incoming.Traverse(Syntax.id);

        public static Try<A> WhereNot<A>(this Try<A> incoming,
            Predicate<A> notPredicate, Func<Exception> errorCallback)
            => incoming.Where(x => !notPredicate(x), errorCallback);
    }
}
