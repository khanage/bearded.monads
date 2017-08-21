using System;
using System.Diagnostics;

namespace Bearded.Monads
{
    public static class OptionUnsafe
    {
        public static OptionUnsafe<A> Return<A>(A value)
        {
            return OptionUnsafe<A>.Return(value);
        }
    }

    [DebuggerStepThrough]
    public abstract class OptionUnsafe<A> : IEquatable<OptionUnsafe<A>>, IEquatable<A>
    {
        public abstract A ForceValue();

        public abstract bool IsSome { get; }

        public static OptionUnsafe<A> None => NoneOptionUnsafe.Instance;

        public static OptionUnsafe<A> Return(A a)
        {
            return new Some(a);
        }

        public abstract OptionUnsafe<B> Map<B>(Func<A, B> mapper);
        public abstract OptionUnsafe<B> FlatMap<B>(Func<A, OptionUnsafe<B>> mapper);

        public abstract void Do(Action<A> valueCallback, Action nullCallback);

        public abstract A Else(Func<A> callbackForNone);

        public abstract OptionUnsafe<CastTarget> Cast<CastTarget>() where CastTarget : A;

        public abstract bool Equals(A other);

        public abstract bool Equals(OptionUnsafe<A> other);

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case OptionUnsafe<A> option: return Equals(option);
                case A a: return Equals(a);
                default: return false;
            }
        }

        public abstract override int GetHashCode();

        public static implicit operator OptionUnsafe<A>(A value) => Return(value);

        public static bool operator true(OptionUnsafe<A> value) => value.IsSome;

        public static bool operator false(OptionUnsafe<A> value) => !value.IsSome;

        public static implicit operator bool(OptionUnsafe<A> value) => value.IsSome;

        public static bool operator !(OptionUnsafe<A> value) => !value.IsSome;

        public static OptionUnsafe<A> operator |(OptionUnsafe<A> left, OptionUnsafe<A> right)
        {
            return left ? left : right;
        }

        public static OptionUnsafe<A> operator |(OptionUnsafe<A> left, Func<OptionUnsafe<A>> right)
        {
            return left ? left : right();
        }

        [DebuggerStepThrough]
        private class Some : OptionUnsafe<A>
        {
            private readonly A force;

            public Some(A force)
            {
                this.force = force;
            }

            public override bool IsSome => true;

            public override A ForceValue() => force;

            public override OptionUnsafe<B> Map<B>(Func<A, B> mapper) => mapper(force);

            public override OptionUnsafe<B> FlatMap<B>(Func<A, OptionUnsafe<B>> mapper) => mapper(force);

            public override void Do(Action<A> valueCallback, Action nullCallback) => valueCallback(force);

            public override A Else(Func<A> callbackForNone) => force;

            public override OptionUnsafe<CastTarget> Cast<CastTarget>()
            {
                if (force is CastTarget)
                    return (CastTarget)force;

                return OptionUnsafe<CastTarget>.None;
            }

            public override bool Equals(A other) => force.Equals(other);

            public override bool Equals(OptionUnsafe<A> other) => other.Equals(force);

            public override int GetHashCode() => force.GetHashCode();

            public override string ToString()
                => "Some(" + force + ")";
        }

        [DebuggerStepThrough]
        private class NoneOptionUnsafe : OptionUnsafe<A>
        {
            private NoneOptionUnsafe() { }

            public static NoneOptionUnsafe Instance { get; } = new NoneOptionUnsafe();

            public override bool IsSome => false;

            public override A ForceValue()
            {
                throw new InvalidOperationException("This does not have a value");
            }

            public override OptionUnsafe<B> Map<B>(Func<A, B> mapper) => OptionUnsafe<B>.None;

            public override OptionUnsafe<B> FlatMap<B>(Func<A, OptionUnsafe<B>> mapper) => OptionUnsafe<B>.None;

            public override void Do(Action<A> valueCallback, Action nullCallback)
            {
                nullCallback();
            }

            public override A Else(Func<A> callbackForNone) => callbackForNone();

            public override OptionUnsafe<CastTarget> Cast<CastTarget>()
            {
                return OptionUnsafe<CastTarget>.None;
            }

            public override bool Equals(A other) => false;

            public override bool Equals(OptionUnsafe<A> other) => !other.IsSome;

            public override int GetHashCode() => 0;

            public override string ToString()
                => "None";
        }
    }
}
