using System;
using System.Collections.Generic;
using System.Linq;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class EitherExtensions
    {
        public static Option<Success> AsOption<Success, Error>(this Either<Success, Error> withError, Action<Error> errorCallback)
        {
            if (withError.IsError)
            {
                errorCallback(withError.AsError.Value);
                return Option<Success>.None;
            }

            return withError.AsSuccess.Value;
        }

        public static Either<Success, Error> AsEither<Success, Error>(this Option<Success> option, Error errorValue)
        {
            if (option.IsSome)
                return option.ForceValue();

            return errorValue;
        }

        public static Either<Success, String> AsEither<Success>(this Option<Success> option, string format, params object[] formatArgs)
        {
            return option.AsEither(string.Format(format, formatArgs));
        }

        public static Either<Success, Error> AsEither<Success, Error>(this Success success) =>
            Either<Success, Error>.CreateSuccess(success);

        public static Either<B, Error> SelectMany<A, B, Error>(
            this Either<A, Error> aOrError,
            Func<A, Either<B, Error>> mapper)
        {
            if (aOrError.IsError) return aOrError.AsError.Value;

            var a = aOrError.AsSuccess.Value;

            return mapper(a);
        }

        public static Either<C, Error> SelectMany<A, B, C, Error>(
            this Either<A, Error> aOrError,
            Func<A, Either<B, Error>> mapToB,
            Func<A, B, C> mapper)
        {
            if (aOrError.IsError) return aOrError.AsError.Value;

            var a = aOrError.AsSuccess.Value;

            var bOrError = mapToB(a);

            if (bOrError.IsError) return bOrError.AsError.Value;

            var b = bOrError.AsSuccess.Value;

            return mapper(a, b);
        }

        public static Either<B, Error> Select<A, Error, B>(this Either<A, Error> either, Func<A, B> projector)
        {
            return either.Map(projector);
        }

        public static Either<A, Error> Where<A, Error>(this Either<A, Error> either,
            Predicate<A> predicate, Func<Error> errorCallback)
        {
            if (either.IsError) return either;
            if (predicate(either.AsSuccess.Value)) return either;
            return errorCallback();
        }

        public static Result Else<A, Error, Result>(this Either<A, Error> either,
            Func<A, Result> happy,
            Func<Error, Result> sad)
        {
            return either.IsSuccess
                ? happy(either.AsSuccess.Value)
                : sad(either.AsError.Value);
        }

        public static Result Unify<Success, Error, Result>(this Either<Success, Error> either,
           Func<Success, Result> successFunc, Func<Error, Result> errorFunc)
        {
            if (either.IsSuccess) return successFunc(either.AsSuccess.Value);
            return errorFunc(either.AsError.Value);
        }

        public static Success Else<Success, Error>(this Either<Success, Error> either,
            Func<Error, Success> callback)
        {
            if (either.IsError) return callback(either.AsError.Value);

            return either.AsSuccess.Value;
        }

        public static Either<A, Error> WhenSuccess<A, Error>(this Either<A, Error> either,
            Action<A> callbackForSuccess)
        {
            if (either.IsSuccess)
            {
                callbackForSuccess(either.AsSuccess.Value);
            }

            return either;
        }

        public static Either<A, Error> WhenError<A, Error>(this Either<A, Error> either,
            Action<Error> callbackForError)
        {
            if (either.IsError)
            {
                callbackForError(either.AsError.Value);
            }

            return either;
        }

        public static A ElseThrow<A, TException>(this Either<A, TException> either)
            where TException : Exception
        {
            return either.Else(exc => { throw exc; });
        }

        public static A ElseThrow<A>(this Either<A, string> either)
        {
            return either.Else(message => { throw new Exception(message); });
        }

        public static Either<A, NewError> MapError<A, Error, NewError>(this Either<A, Error> either, Func<Error, NewError> mapper)
        {
            if (either.IsError)
            {
                return mapper(either.AsError.Value);
            }
            return either.AsSuccess.Value;
        }

        public static Either<A, Error> Flatten<A, Error>(
            this Either<Either<A, Error>, Error> ee)
            => ee.SelectMany(id);

        public static Either<IEnumerable<B>, Error> Traverse<A, B, Error>(
            this IEnumerable<A> enumerable,
            Func<A, Either<B, Error>> callback)
        {
            var mapped = enumerable.Select(callback).ToList();

            var maybeError = mapped.FirstOrNone(x => x.IsError).Select(x => x.AsError.Value);

            return !maybeError.IsSome
                ? Either<IEnumerable<B>,Error>.Create(mapped.Select(x => x.AsSuccess.Value))
                : maybeError.ForceValue();
        }

        public static Either<IEnumerable<A>, Error> Sequence<A, Error>(
            this IEnumerable<Either<A, Error>> incoming)
            => incoming.Traverse(id);

        public static Either<A, Error> WhereNot<A, Error>(this Either<A, Error> incoming,
            Predicate<A> notPredicate, Func<Error> errorCallback)
            => incoming.Where(x => !notPredicate(x), errorCallback);
    }
}
