using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bearded.Monads
{
    public class OptionalApplicative
    {
        public static OptionalApplicative<A> From<A>(Option<A> incoming) =>
            new OptionalApplicative<A>(incoming);
    }

    public static class OptionalApplicativeExtensions
    {
        public static OptionalApplicative<A, B> And<A, B>(this Option<A> a, Option<B> b) =>
            new OptionalApplicative<A, B>(a, b);
    }

    public class OptionalApplicative<A>
    {
        readonly Option<A> a;

        public OptionalApplicative(Option<A> a)
        {
            this.a = a;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<B> Then<B>(Func<A, B> f) => a.Select(f);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B> And<B>(Option<B> b) =>
            new OptionalApplicative<A, B>(a, b);
    }

    public class OptionalApplicative<A, B>
    {
        readonly Option<A> a;
        readonly Option<B> b;

        public OptionalApplicative(Option<A> a, Option<B> b)
        {
            this.a = a;
            this.b = b;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<C> Then<C>(Func<A, B, C> func) =>
            from a_ in a
            from b_ in b
            select func(a_, b_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B, C> And<C>(Option<C> c) =>
            new OptionalApplicative<A, B, C>(a, b, c);
    }

    public class OptionalApplicative<A, B, C>
    {
        readonly Option<A> a;
        readonly Option<B> b;
        readonly Option<C> c;

        public OptionalApplicative(Option<A> a, Option<B> b, Option<C> c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<D> Then<D>(Func<A, B, C, D> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            select func(a_, b_, c_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B, C, D> And<D>(Option<D> d) =>
            new OptionalApplicative<A, B, C, D>(a, b, c, d);
    }

    public class OptionalApplicative<A, B, C, D>
    {
        readonly Option<A> a;
        readonly Option<B> b;
        readonly Option<C> c;
        readonly Option<D> d;

        public OptionalApplicative(Option<A> a, Option<B> b, Option<C> c, Option<D> d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<E> Then<E>(Func<A, B, C, D, E> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            select func(a_, b_, c_, d_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B, C, D, E> And<E>(Option<E> e) =>
            new OptionalApplicative<A, B, C, D, E>(a, b, c, d, e);
    }

    public class OptionalApplicative<A, B, C, D, E>
    {
        readonly Option<A> a;
        readonly Option<B> b;
        readonly Option<C> c;
        readonly Option<D> d;
        readonly Option<E> e;

        public OptionalApplicative(Option<A> a, Option<B> b, Option<C> c, Option<D> d, Option<E> e)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<F> Then<F>(Func<A, B, C, D, E, F> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            select func(a_, b_, c_, d_, e_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B, C, D, E, F> And<F>(Option<F> f) =>
            new OptionalApplicative<A, B, C, D, E, F>(a, b, c, d, e, f);
    }

    public class OptionalApplicative<A, B, C, D, E, F>
    {
        readonly Option<A> a;
        readonly Option<B> b;
        readonly Option<C> c;
        readonly Option<D> d;
        readonly Option<E> e;
        readonly Option<F> f;

        public OptionalApplicative(Option<A> a, Option<B> b, Option<C> c, Option<D> d, Option<E> e, Option<F> f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
        }

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<G> Then<G>(Func<A, B, C, D, E, F, G> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            from f_ in f
            select func(a_, b_, c_, d_, e_, f_);

        [DebuggerStepThrough,MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionalApplicative<A, B, C, D, E, F, G> And<G>(Option<G> g) =>
            new OptionalApplicative<A, B, C, D, E, F, G>(a, b, c, d, e, f, g);
    }

    public class OptionalApplicative<A, B, C, D, E, F, G>
    {
        readonly Option<A> a;
        readonly Option<B> b;
        readonly Option<C> c;
        readonly Option<D> d;
        readonly Option<E> e;
        readonly Option<F> f;
        readonly Option<G> g;

        public OptionalApplicative(Option<A> a, Option<B> b, Option<C> c, Option<D> d, Option<E> e, Option<F> f, Option<G> g)
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
        public Option<H> Then<H>(Func<A, B, C, D, E, F, G, H> func) =>
            from a_ in a
            from b_ in b
            from c_ in c
            from d_ in d
            from e_ in e
            from f_ in f
            from g_ in g
            select func(a_, b_, c_, d_, e_, f_, g_);

        //public OptionalApplicative<A, B, C, D, E, F, G, H> And<H>(Option<H> h) =>
        //    new OptionalApplicative<A, B, C, D, E, F, G, H>(a, b, c, d, e, f, g, h);
    }
}