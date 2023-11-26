using System;
using System.Diagnostics;

namespace Bearded.Monads
{
    public static class Option
    {
        public static Option<A> Return<A>(A value)
        {
            return Option<A>.Return(value);
        }
    }

    [DebuggerStepThrough]
    public abstract class Option<A> : IEquatable<Option<A>>, IEquatable<A>
    {
        public abstract A ForceValue();

        public abstract bool IsSome { get; }

        public static Option<A> None => NoneOption.Instance;

        public static Option<A> Return(A a)
        {
            return ReferenceEquals(null, a) ? None : new Some(a);
        }

        public abstract Option<B> Map<B>(Func<A, B> mapper);

        public abstract void Do(Action<A> valueCallback, Action nullCallback);

        public abstract A Else(Func<A> callbackForNone);

        public abstract A Else(A valueForNone);

        public abstract Option<CastTarget> Cast<CastTarget>() where CastTarget : A;

        public abstract bool Equals(A other);

        public abstract bool Equals(Option<A> other);

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Option<A> option: return Equals(option);
                case A a: return Equals(a);
                default: return false;
            }
        }

        public abstract override int GetHashCode();

        public static implicit operator Option<A>(A value) => Return(value);

        public static bool operator true(Option<A> value) => value.IsSome;

        public static bool operator false(Option<A> value) => !value.IsSome;

        public static implicit operator bool(Option<A> value) => value.IsSome;

        public static bool operator !(Option<A> value) => !value.IsSome;

        public static Option<A> operator |(Option<A> left, Option<A> right)
        {
            return left ? left : right;
        }

        public static Option<A> operator |(Option<A> left, Func<Option<A>> right)
        {
            return left ? left : right();
        }

        [DebuggerStepThrough]
        private class Some : Option<A>
        {
            private readonly A force;

            public Some(A force)
            {
                this.force = force;
            }

            public override bool IsSome => true;

            public override A ForceValue() => force;

            public override Option<B> Map<B>(Func<A, B> mapper) => mapper(force);

            public override void Do(Action<A> valueCallback, Action nullCallback) => valueCallback(force);

            public override A Else(Func<A> callbackForNone) => force;

            public override A Else(A valueForNone) => force;

            public override Option<CastTarget> Cast<CastTarget>()
            {
                if (force is CastTarget)
                    return (CastTarget)force;

                return Option<CastTarget>.None;
            }

            public override bool Equals(A other) => force.Equals(other);

            public override bool Equals(Option<A> other) => other.Equals(force);

            public override int GetHashCode() => force.GetHashCode();

            public override string ToString()
                => "Some(" + force + ")";
        }

        [DebuggerStepThrough]
        private class NoneOption : Option<A>
        {
            private NoneOption() { }

            public static NoneOption Instance { get; } = new NoneOption();

            public override bool IsSome => false;

            public override A ForceValue()
            {
                throw new InvalidOperationException("This does not have a value");
            }

            public override Option<B> Map<B>(Func<A, B> mapper)
            {
                return Option<B>.None;
            }

            public override void Do(Action<A> valueCallback, Action nullCallback)
            {
                nullCallback();
            }

            public override A Else(Func<A> callbackForNone) => callbackForNone();

            public override A Else(A valueForNone) => valueForNone;

            public override Option<CastTarget> Cast<CastTarget>()
            {
                return Option<CastTarget>.None;
            }

            public override bool Equals(A other) => false;

            public override bool Equals(Option<A> other) => !other.IsSome;

            public override int GetHashCode() => 0;

            public override string ToString()
                => "None";
        }
    }
}
