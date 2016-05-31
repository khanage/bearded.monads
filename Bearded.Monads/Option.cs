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

    public abstract class Option<A> : IEquatable<Option<A>>, IEquatable<A>
    {
        public abstract A ForceValue();
        public abstract bool Equals(Option<A> other);
        public abstract bool IsSome { [DebuggerStepThrough]get; }

        public static Option<A> None => NoneOption.Instance;

        [DebuggerStepThrough]
        public static Option<A> Return(A a)
        {
            return new Some(a);
        }

        [DebuggerStepThrough]
        public abstract Option<B> Map<B>(Func<A, B> mapper);
        [DebuggerStepThrough]
        public abstract void Do(Action<A> callback);
        [DebuggerStepThrough]
        public abstract void Do(Action<A> valueCallback, Action nullCallback);

        [DebuggerStepThrough]
        public A Else(Func<A> callbackForNone)
        {
            if (this.IsSome)
            {
                return this.ForceValue();
            }
            return callbackForNone();
        }

        [DebuggerStepThrough]
        public A ElseDefault()
        {
            if (this.IsSome)
            {
                return this.ForceValue();
            }
            return default(A);
        }

        [DebuggerStepThrough]
        public Option<B> SelectMany<B>(Func<A, Option<B>> mapper)
        {
            return this.IsSome ? mapper(this.ForceValue()) : Option<B>.None;
        }

        [DebuggerStepThrough]
        public Option<Tuple<A, B>> Concat<B>(Option<B> ob)
        {
            var emptyResult = Option<Tuple<A, B>>.None;

            if (!this.IsSome)
            {
                return emptyResult;
            }
            if (!ob.IsSome)
            {
                return emptyResult;
            }

            return new Tuple<A, B>(this.ForceValue(), ob.ForceValue());
        }

        [DebuggerStepThrough]
        public Option<A> Where(Predicate<A> pred)
        {
            if (this.IsSome && pred(this.ForceValue()))
            {
                return this;
            }

            return NoneOption.Instance;
        }

        [DebuggerStepThrough]
        public Option<A> Empty(Action nullCallback)
        {
            if (!this.IsSome)
            {
                nullCallback();
            }

            return this;
        }

        [DebuggerStepThrough]
        public Option<A> WhenSome(Action<A> callback)
        {
            if (this.IsSome)
            {
                callback(this.ForceValue());
            }

            return this;
        }

        [DebuggerStepThrough]
        public Option<A> WhenNone(Action callback)
        {
            if (!this.IsSome)
            {
                callback();
            }

            return this;
        }

        [DebuggerStepThrough]
        public Option<CastTarget> Cast<CastTarget>()
            where CastTarget : A
        {
            return SelectMany(ct => ct.MaybeCast<CastTarget>());
        }

        public static implicit operator Option<A>(A value)
        {
            if (!(typeof (A).IsValueType) && Equals(null, value))
                return None;

            return Return(value);
        }

        public static bool operator true(Option<A> value)
        {
            return value.IsSome;
        }

        public static bool operator false(Option<A> value)
        {
            return !value.IsSome;
        }

        public static implicit operator bool(Option<A> value)
        {
            return value.IsSome;
        }

        public static bool operator !(Option<A> value)
        {
            return !value.IsSome;
        }

        public static Option<A> operator |(Option<A> left, Option<A> right)
        {
            if (left)
                return left;
            if (right)
                return right;

            return None;
        }

        public bool Equals(A other)
        {
            return this.IsSome && this.ForceValue().Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Option<A>)
            {
                return Equals(obj as Option<A>);
            }
            if (obj is A)
            {
                return Equals((A)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.IsSome ? this.ForceValue().GetHashCode() : 0;
        }

        [DebuggerDisplay("Some({force})")]
        class Some : Option<A>
        {
            readonly A force;

            public Some(A force)
            {
                this.force = force;
            }

            public override bool IsSome
            {
                [DebuggerStepThrough]
                get { return true; }
            }

            public override A ForceValue()
            {
                return this.force;
            }

            public override Option<B> Map<B>(Func<A, B> mapper)
            {
                return mapper(this.ForceValue());
            }

            public override void Do(Action<A> callback)
            {
                callback(this.force);
            }

            public override void Do(Action<A> valueCallback, Action nullCallback)
            {
                valueCallback(this.force);
            }

            public override bool Equals(Option<A> other)
            {
                return other.IsSome && this.ForceValue().Equals(other.ForceValue());
            }
        }

        [DebuggerDisplay("None")]
        class NoneOption : Option<A>
        {
            NoneOption() { }

            public static NoneOption Instance { get; } = new NoneOption();

            public override bool IsSome
            {
                [DebuggerStepThrough]
                get { return false; }
            }

            public override A ForceValue()
            {
                throw new InvalidOperationException("This does not have a value");
            }

            public override Option<B> Map<B>(Func<A, B> mapper)
            {
                return Option<B>.None;
            }

            public override void Do(Action<A> callback) { }

            public override void Do(Action<A> valueCallback, Action nullCallback)
            {
                nullCallback();
            }

            public override bool Equals(Option<A> other)
            {
                return ReferenceEquals(this, other);
            }
        }
    }
}
