using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bearded.Monads
{
    public class EitherApplicative
    {
        public static EitherApplicative<A,Error> From<A,Error>(Either<A,Error> incoming) =>
            new EitherApplicative<A,Error>(incoming);
    }

    public static class EitherApplicativeExtensions
    {
        public static EitherApplicative<A,B,Error> And<A,B,Error>(this Either<A,Error> a, Either<B,Error> b) =>
            new EitherApplicative<A,B,Error>(a, b);
    }

    public class EitherApplicative<A,Error>
    {
        readonly Either<A,Error> a;

        public EitherApplicative(Either<A,Error> a)
        {
            this.a = a;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<B,Error> Then<B>(Func<A,B> f) => a.Select(f);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,Error> And<B>(Either<B, Error> b) =>
            new EitherApplicative<A,B,Error>(a, b);
    }

    public class EitherApplicative<A,B,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b)
        {
            this.a = a;
            this.b = b;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<C,Error> Then<C>(Func<A, B, C> func) =>
            from a_ in a
            from b_ in b
            select func(a_, b_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,C,Error> And<C>(Either<C,Error> c) =>
            new EitherApplicative<A,B,C,Error>(a, b, c);
    }

    public class EitherApplicative<A,B,C,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;
        readonly Either<C,Error> c;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b, Either<C,Error> c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<D,Error> Then<D>(Func<A, B, C, D> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            select func(a_, b_, c_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,C,D,Error> And<D>(Either<D,Error> d) =>
            new EitherApplicative<A,B,C,D,Error>(a, b, c, d);
    }

    public class EitherApplicative<A,B,C,D,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;
        readonly Either<C,Error> c;
        readonly Either<D,Error> d;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b, Either<C,Error> c, Either<D,Error> d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<E,Error> Then<E>(Func<A, B, C, D, E> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            select func(a_, b_, c_, d_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,C,D,E,Error> And<E>(Either<E,Error> e) =>
            new EitherApplicative<A,B,C,D,E,Error>(a, b, c, d, e);
    }

    public class EitherApplicative<A,B,C,D,E,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;
        readonly Either<C,Error> c;
        readonly Either<D,Error> d;
        readonly Either<E,Error> e;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b, Either<C,Error> c, Either<D,Error> d, Either<E,Error> e)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<F,Error> Then<F>(Func<A, B, C, D, E, F> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            select func(a_, b_, c_, d_, e_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,C,D,E,F,Error> And<F>(Either<F,Error> f) =>
            new EitherApplicative<A,B,C,D,E,F,Error>(a, b, c, d, e, f);
    }

    public class EitherApplicative<A,B,C,D,E,F,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;
        readonly Either<C,Error> c;
        readonly Either<D,Error> d;
        readonly Either<E,Error> e;
        readonly Either<F,Error> f;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b, Either<C,Error> c, Either<D,Error> d, Either<E,Error> e, Either<F,Error> f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<G,Error> Then<G>(Func<A, B, C, D, E, F, G> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            from f_ in f
            select func(a_, b_, c_, d_, e_, f_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EitherApplicative<A,B,C,D,E,F,G,Error> And<G>(Either<G,Error> g) =>
            new EitherApplicative<A,B,C,D,E,F,G,Error>(a, b, c, d, e, f, g);
    }

    public class EitherApplicative<A,B,C,D,E,F,G,Error>
    {
        readonly Either<A,Error> a;
        readonly Either<B,Error> b;
        readonly Either<C,Error> c;
        readonly Either<D,Error> d;
        readonly Either<E,Error> e;
        readonly Either<F,Error> f;
        readonly Either<G,Error> g;

        public EitherApplicative(Either<A,Error> a, Either<B,Error> b, Either<C,Error> c, Either<D,Error> d, Either<E,Error> e, Either<F,Error> f, Either<G,Error> g)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Either<H,Error> Then<H>(Func<A, B, C, D, E, F, G, H> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            from f_ in f
            from g_ in g
            select func(a_, b_, c_, d_, e_, f_, g_);
    }
}
