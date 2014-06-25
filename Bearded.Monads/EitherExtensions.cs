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
                return option.Value;

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
    }
}