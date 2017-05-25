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
        public static void noop<A>(A a) {}

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
    }
}
