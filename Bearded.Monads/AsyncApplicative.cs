#if NET45
using System;
using System.Threading.Tasks;

namespace Bearded.Monads
{
    public static class AsyncApplicative
    {
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

    public class AsyncApplicative<A>
    {
        readonly Task<A> callbackA;

        public AsyncApplicative(Task<A> callbackA)
        {
            this.callbackA = callbackA;
        }

        public AsyncApplicative<A, B> And<B>(Task<B> nextCallback) =>
            AsyncApplicative.Asynquence(callbackA, nextCallback);

        public Task<B> Select<B>(Func<A, B> map) =>
            Task.WhenAll(callbackA.Run())
                .ContinueWith(t =>
                    map(callbackA.Result)
                );
    }

    public class AsyncApplicative<A, B>
    {
        readonly Task<A> callbackA;
        readonly Task<B> callbackB;

        public AsyncApplicative(Task<A> callbackA, Task<B> callbackB)
        {
            this.callbackA = callbackA;
            this.callbackB = callbackB;
        }

        public AsyncApplicative<A, B, C> And<C>(Task<C> callbackC) =>
            AsyncApplicative.Asynquence(callbackA, callbackB, callbackC);

        public Task<C> Select<C>(Func<A, B, C> map) =>
            Task.WhenAll(callbackA.Run(), callbackB.Run())
                .ContinueWith(t =>
                    map(callbackA.Result, callbackB.Result)
                );
    }

    public class AsyncApplicative<A, B, C>
    {
        readonly Task<A> callbackA;
        readonly Task<B> callbackB;
        readonly Task<C> callbackC;

        public AsyncApplicative(Task<A> callbackA, Task<B> callbackB, Task<C> callbackC)
        {
            this.callbackA = callbackA;
            this.callbackB = callbackB;
            this.callbackC = callbackC;
        }

        public AsyncApplicative<A, B, C, D> And<D>(Task<D> callbackD) =>
            AsyncApplicative.Asynquence(callbackA, callbackB, callbackC, callbackD);

        public Task<D> Select<D>(Func<A, B, C, D> map) =>
            Task.WhenAll(callbackA.Run(), callbackB.Run(), callbackC.Run())
                .ContinueWith(t =>
                    map(callbackA.Result, callbackB.Result, callbackC.Result)
                );
    }

    public class AsyncApplicative<A, B, C, D>
    {
        readonly Task<A> callbackA;
        readonly Task<B> callbackB;
        readonly Task<C> callbackC;
        readonly Task<D> callbackD;

        public AsyncApplicative(Task<A> callbackA, Task<B> callbackB, Task<C> callbackC, Task<D> callbackD)
        {
            this.callbackA = callbackA;
            this.callbackB = callbackB;
            this.callbackC = callbackC;
            this.callbackD = callbackD;
        }

        public AsyncApplicative<A, B, C, D, E> And<E>(Task<E> callbackE) =>
            AsyncApplicative.Asynquence(callbackA, callbackB, callbackC, callbackD, callbackE);

        public Task<E> Select<E>(Func<A, B, C, D, E> map) =>
            Task.WhenAll(callbackA.Run(), callbackB.Run(), callbackC.Run(), callbackD.Run())
                .ContinueWith(t =>
                    map(callbackA.Result, callbackB.Result, callbackC.Result, callbackD.Result)
                );
    }

    public class AsyncApplicative<A, B, C, D, E>
    {
        readonly Task<A> callbackA;
        readonly Task<B> callbackB;
        readonly Task<C> callbackC;
        readonly Task<D> callbackD;
        readonly Task<E> callbackE;

        public AsyncApplicative(Task<A> callbackA, Task<B> callbackB, Task<C> callbackC, Task<D> callbackD,
            Task<E> callbackE)
        {
            this.callbackA = callbackA;
            this.callbackB = callbackB;
            this.callbackC = callbackC;
            this.callbackD = callbackD;
            this.callbackE = callbackE;
        }

        public AsyncApplicative<A, B, C, D, E, F> And<F>(Task<F> callbackF) =>
            AsyncApplicative.Asynquence(callbackA, callbackB, callbackC, callbackD, callbackE, callbackF);

        public Task<F> Select<F>(Func<A, B, C, D, E, F> map) =>
            Task.WhenAll(callbackA.Run(), callbackB.Run(), callbackC.Run(), callbackD.Run(), callbackE.Run())
                .ContinueWith(t =>
                    map(callbackA.Result, callbackB.Result, callbackC.Result, callbackD.Result, callbackE.Result)
                );
    }

    public class AsyncApplicative<A, B, C, D, E, F>
    {
        readonly Task<A> callbackA;
        readonly Task<B> callbackB;
        readonly Task<C> callbackC;
        readonly Task<D> callbackD;
        readonly Task<E> callbackE;
        readonly Task<F> callbackF;

        public AsyncApplicative(Task<A> callbackA, Task<B> callbackB, Task<C> callbackC, Task<D> callbackD,
            Task<E> callbackE, Task<F> callbackF)
        {
            this.callbackA = callbackA;
            this.callbackB = callbackB;
            this.callbackC = callbackC;
            this.callbackD = callbackD;
            this.callbackE = callbackE;
            this.callbackF = callbackF;
        }

        // Really??? PR if you need this though
        //public AsyncApplicative<A, B, C, D, E, F, G> And<G>(Task<G> callbackG) =>
        //    AsyncApplicative.Asynquence(callbackA, callbackB, callbackC, callbackD, callbackE, callbackF, callbackG);

        public Task<G> Select<G>(Func<A, B, C, D, E, F, G> map) =>
            Task.WhenAll(callbackA.Run(), callbackB.Run(), callbackC.Run(), callbackD.Run(), callbackE.Run(),
                callbackF.Run())
                .ContinueWith(t =>
                    map(callbackA.Result, callbackB.Result, callbackC.Result, callbackD.Result, callbackE.Result,
                        callbackF.Result)
                );
    }
}
#endif