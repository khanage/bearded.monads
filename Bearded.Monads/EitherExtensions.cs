using System;

namespace Bearded.Monads
{
    public static class EitherExtensions
    {
        public static Option<Success> AsOption<Success, Error>(this EitherSuccessOrError<Success, Error> withError, Action<Error> errorCallback)
        {
            if (withError.IsError)
            {
                errorCallback(withError.AsError.Value);
                return Option<Success>.None;
            }

            return withError.AsSuccess.Value;
        }

        public static EitherSuccessOrError<Success, Error> AsEither<Success, Error>(this Option<Success> option, Error errorValue)
        {
            if (option.IsSome)
                return option.ForceValue();

            return errorValue;
        }

        public static EitherSuccessOrError<Success, String> AsEither<Success>(this Option<Success> option, string format, params object[] formatArgs)
        {
            return option.AsEither(string.Format(format, formatArgs));
        }

        public static EitherSuccessOrError<B, Error> SelectMany<A, B, Error>(
            this EitherSuccessOrError<A, Error> aOrError,
            Func<A, EitherSuccessOrError<B, Error>> mapper)
        {
            if (aOrError.IsError) return aOrError.AsError.Value;

            var a = aOrError.AsSuccess.Value;

            return mapper(a);
        }

        public static EitherSuccessOrError<C, Error> SelectMany<A, B, C, Error>(
            this EitherSuccessOrError<A, Error> aOrError,
            Func<A, EitherSuccessOrError<B, Error>> mapToB,
            Func<A, B, C> mapper)
        {
            if (aOrError.IsError) return aOrError.AsError.Value;

            var a = aOrError.AsSuccess.Value;

            var bOrError = mapToB(a);

            if (bOrError.IsError) return bOrError.AsError.Value;

            var b = bOrError.AsSuccess.Value;

            return mapper(a, b);
        }

        public static EitherSuccessOrError<B, Error> Select<A, Error, B>(this EitherSuccessOrError<A, Error> either, Func<A, B> projector)
        {
            return either.Map(projector);
        }

        public static EitherSuccessOrError<A, Error> Where<A, Error>(this EitherSuccessOrError<A, Error> either,
            Predicate<A> predicate, Func<Error> errorCallback)
        {
            if (either.IsError) return either;
            if (predicate(either.AsSuccess.Value)) return either;
            return errorCallback();
        }

        public static Result Else<A, Error, Result>(this EitherSuccessOrError<A, Error> either, 
            Func<A, Result> happy,
            Func<Error, Result> sad)
        {
            return either.IsSuccess 
                ? happy(either.AsSuccess.Value)
                : sad(either.AsError.Value);
        }

        public static EitherSuccessOrError<Result, Exception> SafeCallback<Incoming, Result>(this Incoming item, 
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

        public static Result Unify<Success, Error, Result>(this EitherSuccessOrError<Success, Error> either,
           Func<Success, Result> successFunc, Func<Error, Result> errorFunc)
        {
            if (either.IsSuccess) return successFunc(either.AsSuccess.Value);
            return errorFunc(either.AsError.Value);
        }
    }
}