using System;
using System.Threading.Tasks;

namespace Bearded.Monads
{
    /// <summary>
    /// This contains syntax extensions, and is intended to be imported
    /// statically, e.g. `using static Bearded.Monads.Syntax`
    /// </summary>
    public static class Syntax
    {
        public static A id<A>(A a) => a;

        public static void noop<A>(A a)
        {
        }

        public static Func<A, C> comp<A, B, C>(Func<A, B> f1, Func<B, C> f2)
            => a => f2(f1(a));

        public static AlwaysSyntax<A> always<A>(Func<A> val) => new AlwaysSyntax<A>(val);
        public static AlwaysSyntax<A> always<A>(A literalVal) => new AlwaysSyntax<A>(() => literalVal);
        public class AlwaysSyntax<A>
        {
            private readonly Func<A> val;
            public AlwaysSyntax(Func<A> val) => this.val = val;

            public A func<B>(B _) => val();
        }

        public static OptionalApplicative<A> Optionally<A>(Option<A> option)
            => new OptionalApplicative<A>(option);

        public static EitherApplicative<A,Error> EitherArg<A,Error>(Either<A,Error> either)
            => new EitherApplicative<A,Error>(either);

        public static AsyncApplicative<A> Asynquence<A>(Task<A> callback) =>
            new AsyncApplicative<A>(callback);

        public static AsyncApplicative<A, B> Asynquence<A, B>(Task<A> callbackA, Task<B> callbackB) =>
            new AsyncApplicative<A, B>(callbackA, callbackB);

        public static AsyncApplicative<A, B, C> Asynquence<A, B, C>(Task<A> callbackA, Task<B> callbackB,
            Task<C> callbackC) =>
            new AsyncApplicative<A, B, C>(callbackA, callbackB, callbackC);

        public static AsyncApplicative<A, B, C, D> Asynquence<A, B, C, D>(Task<A> callbackA, Task<B> callbackB,
            Task<C> callbackC, Task<D> callbackD) =>
            new AsyncApplicative<A, B, C, D>(callbackA, callbackB, callbackC, callbackD);

        public static AsyncApplicative<A, B, C, D, E> Asynquence<A, B, C, D, E>(Task<A> callbackA, Task<B> callbackB,
            Task<C> callbackC, Task<D> callbackD, Task<E> callbackE) =>
            new AsyncApplicative<A, B, C, D, E>(callbackA, callbackB, callbackC, callbackD, callbackE);

        public static AsyncApplicative<A, B, C, D, E, F> Asynquence<A, B, C, D, E, F>(Task<A> callbackA,
            Task<B> callbackB, Task<C> callbackC, Task<D> callbackD, Task<E> callbackE, Task<F> callbackF) =>
            new AsyncApplicative<A, B, C, D, E, F>(callbackA, callbackB, callbackC, callbackD, callbackE, callbackF);

        public static Try<A> Try<A>(Func<Try<A>> f)
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        public static Try<A> Try<A>(Func<A> f)
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
