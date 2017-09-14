using System;

namespace Bearded.Monads
{
    public abstract class Try<Success> : IEquatable<Try<Success>>
    {
        public static Try<Success> Create(Success success) => new TryContainer(success);
        public static Try<Success> Create(Exception e) => new ExceptionContainer(e);

        public static implicit operator Try<Success>(Success success)
            => Create(success);

        public static implicit operator Try<Success>(Exception exception)
            => Create(exception);

        public bool IsError => this is ExceptionContainer;
        public bool IsSuccess => this is TryContainer;

        internal TryContainer AsSuccess() => this as TryContainer;
        internal ExceptionContainer AsError() => this as ExceptionContainer;

        public Try<Next> Map<Next>(Func<Success, Next> f) => MapImpl(f);

        protected abstract Try<Next> MapImpl<Next>(Func<Success, Next> f);

        public abstract void Do(Action<Success> successCallback, Action<Exception> errorCallback);

        internal class TryContainer : Try<Success>
        {
            public Success Value { get; }
            public TryContainer(Success success) => Value = success;

            protected override Try<Next> MapImpl<Next>(Func<Success, Next> f)
            {
                try
                {
                    var result = f(Value);

                    return Try<Next>.Create(result);
                }
                catch (Exception e)
                {
                    return Try<Next>.Create(e);
                }
            }

            public override void Do(Action<Success> successCallback, Action<Exception> errorCallback)
            {
                successCallback(Value);
            }
        }

        internal class ExceptionContainer : Try<Success>
        {
            public Exception Value { get; }
            public ExceptionContainer(Exception e) => Value = e;

            protected override Try<Next> MapImpl<Next>(Func<Success, Next> f)
                => Try<Next>.Create(Value);

            public override void Do(Action<Success> successCallback, Action<Exception> errorCallback)
            {
                errorCallback(Value);
            }
        }

        public bool Equals(Try<Success> other)
        {
            if (other.IsError && IsError)
                return other.AsError().Value == AsError().Value;
            if (other.IsSuccess && IsSuccess)
                return other.AsSuccess().Value.Equals(AsSuccess().Value);

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Try<Success>) obj);
        }

        public override int GetHashCode() => IsSuccess
            ? 387 * AsSuccess().Value.GetHashCode()
            : AsError().Value.GetHashCode();

        public static bool operator ==(Try<Success> left, Try<Success> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Try<Success> left, Try<Success> right)
        {
            return !Equals(left, right);
        }
    }
}
