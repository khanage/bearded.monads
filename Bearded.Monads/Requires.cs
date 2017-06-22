using System;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class Requires<Dependency>
    {
        public static Requires<Dependency, Result> In<Result>(Func<Dependency, Result> f)
            => Requires<Dependency, Result>.Pure(f);
    }

    public class Requires<A, B>
    {
        private readonly Func<A, B> func;

        public static Requires<A, B> Pure(Func<A, B> f)
            => new Requires<A, B>(f);

        public Requires(Func<A, B> func)
        {
            this.func = func;
        }

        public B Run(A input) => func(input);

        public Requires<A, C> Select<C>(Func<B, C> f)
            => new Requires<A, C>(comp(func, f));

        public Requires<A, C> Join<C>(Func<B, Requires<A, C>> f)
            => new Requires<A, C>(a => comp(func, f).Invoke(a).Run(a));

        public Requires<A, B> Do(Action<B> action)
            => Do((a, b) => action(b));

        public Requires<A, B> Do(Action<A, B> action)
            => Requires<A>.In(a =>
            {
                var b = func(a);
                action(a, b);

                return b;
            });
    }

    public static class RequiresExtensions
    {
        public static Requires<A, D> SelectMany<A, B, C, D>(
            this Requires<A, B> requiresB,
            Func<B, Requires<A, C>> mapC,
            Func<B, C, D> mapper
        ) => Requires<A, D>.Pure(a =>
        {
            var b = requiresB.Run(a);
            var requiresC = mapC(b);

            return mapper(b, requiresC.Run(a));
        });
    }
}
