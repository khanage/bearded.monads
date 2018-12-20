using System;

namespace Bearded.Monads
{
    public static class Alternative
    {
        public static Either<Success, Error> Alternatively<Success, Error>
            (this Either<Success, Error> initial, Func<Either<Success, Error>> subsequent) =>
            initial.Unify(Either<Success, Error>.CreateSuccess, _ => subsequent());

        public static Either<Success, Error> Alternatively<Success, Error>
            (this Either<Success, Error> initial, Either<Success, Error> subsequent) =>
            initial.Alternatively(() => subsequent);
        
        public static Try<Success> Alternatively<Success>
            (this Try<Success> initial, Func<Try<Success>> subsequent) =>
            initial.Unify(Try<Success>.Create, _ => subsequent());

        public static Try<Success> Alternatively<Success>
            (this Try<Success> initial, Try<Success> subsequent) =>
            initial.Alternatively(() => subsequent);
    }
}