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
        public abstract A Value { [DebuggerStepThrough]get; }
        public abstract bool Equals(Option<A> other);
        public abstract bool IsSome { [DebuggerStepThrough]get; }

        public static Option<A> None { get { return NoneOption<A>.Instance; } }

        [DebuggerStepThrough]
        public static Option<A> Return(A a)
        {
            return new Some<A>(a);
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
                return this.Value;
            }
            return callbackForNone();
        }

        [DebuggerStepThrough]
        public A ElseDefault()
        {
            if (this.IsSome)
            {
                return this.Value;
            }
            return default(A);
        }

        [DebuggerStepThrough]
        public Option<B> SelectMany<B>(Func<A, Option<B>> mapper)
        {
            return this.IsSome ? mapper(this.Value) : NoneOption<B>.Instance;
        }

        [DebuggerStepThrough]
        public Option<Tuple<A, B>> Concat<B>(Option<B> ob)
        {
            var emptyResult = NoneOption<Tuple<A, B>>.Instance;

            if (!this.IsSome)
            {
                return emptyResult;
            }
            if (!ob.IsSome)
            {
                return emptyResult;
            }

            return new Tuple<A, B>(this.Value, ob.Value);
        }

        [DebuggerStepThrough]
        public Option<A> Where(Predicate<A> pred)
        {
            if (this.IsSome && pred(this.Value))
            {
                return this;
            }

            return NoneOption<A>.Instance;
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
                callback(this.Value);
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

        public bool Equals(A other)
        {
            return this.IsSome && this.Value.Equals(other);
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
            return this.IsSome ? this.Value.GetHashCode() : 0;
        }

        [DebuggerDisplay("Some({Value})")]
        class Some<B> : Option<B>
        {
            readonly B value;

            public Some(B value)
            {
                this.value = value;
            }

            public override bool IsSome
            {
                [DebuggerStepThrough]
                get { return true; }
            }

            public override B Value
            {
                [DebuggerStepThrough]
                get { return this.value; }
            }

            public override Option<C> Map<C>(Func<B, C> mapper)
            {
                return mapper(this.Value);
            }

            public override void Do(Action<B> callback)
            {
                callback(this.value);
            }

            public override void Do(Action<B> valueCallback, Action nullCallback)
            {
                valueCallback(this.value);
            }

            public override bool Equals(Option<B> other)
            {
                return other.IsSome && this.Value.Equals(other.Value);
            }
        }

        [DebuggerDisplay("None")]
        class NoneOption<B> : Option<B>
        {
            NoneOption() { }

            static readonly NoneOption<B> instance = new NoneOption<B>();
            public static NoneOption<B> Instance
            {
                get { return instance; }
            }

            public override bool IsSome
            {
                [DebuggerStepThrough]
                get { return false; }
            }

            public override B Value
            {
                [DebuggerStepThrough]
                get { throw new InvalidOperationException("This does not have a value"); }
            }

            public override Option<C> Map<C>(Func<B, C> mapper)
            {
                return NoneOption<C>.Instance;
            }

            public override void Do(Action<B> callback) { }

            public override void Do(Action<B> valueCallback, Action nullCallback)
            {
                nullCallback();
            }

            public override bool Equals(Option<B> other)
            {
                return ReferenceEquals(this, other);
            }
        }
    }
}
