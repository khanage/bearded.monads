using System;
using System.Diagnostics;

namespace Bearded.Monads
{
    [Obsolete("EitherSuccessOrError has been renamed to Either")]
    public abstract class EitherSuccessOrError<Success, Error> {}

    public abstract class Either<Success, Error> : IEquatable<Either<Success, Error>>
    {
        internal ErrorContainer AsError => this as ErrorContainer;
        internal SuccessContainer AsSuccess => this as SuccessContainer;

        public bool IsError => this is ErrorContainer;
        public bool IsSuccess => this is SuccessContainer;

        [DebuggerStepThrough]
        public static Either<Success, Error> Create(Error value) =>
            new ErrorContainer(value);
        [DebuggerStepThrough]
        public static Either<Success, Error> CreateError(Error value) =>
            new ErrorContainer(value);
        
        [DebuggerStepThrough]
        public static Either<Success, Error> Create(Success value) =>
            new SuccessContainer(value);
        [DebuggerStepThrough]
        public static Either<Success, Error> CreateSuccess(Success value) =>
            new SuccessContainer(value);        
        
        #region Abstract methods
        [DebuggerStepThrough]
        public abstract Either<NextSuccess,Error> Map<NextSuccess>(Func<Success,NextSuccess> mapper);

        [DebuggerStepThrough]
        public abstract void Do(Action<Success> successCallback, Action<Error> errorCallback);
        #endregion

        #region Operators
        [DebuggerStepThrough]
        public static implicit operator Either<Success, Error>(Error error) =>
            new ErrorContainer(error);

        [DebuggerStepThrough]
        public static implicit operator Either<Success, Error>(Success success) =>
            new SuccessContainer(success);

        #endregion

        public bool Equals(Either<Success, Error> other)
        {
            if (this.IsSuccess && other.IsSuccess)
                return this.AsSuccess.Value.Equals(other.AsSuccess.Value);

            if (this.IsError && other.IsError)
                return this.AsError.Value.Equals(other.AsError.Value);

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Either<Success, Error>) obj);
        }

        public override int GetHashCode()
        {
            if (this.IsSuccess)
                return this.AsSuccess.Value.GetHashCode();

            return this.AsError.Value.GetHashCode();
        }

        public static bool operator ==(Either<Success, Error> left, Either<Success, Error> right) =>
            Equals(left, right);

        public static bool operator !=(Either<Success, Error> left, Either<Success, Error> right) =>
            !Equals(left, right);

        #region Implementations
        internal class ErrorContainer : Either<Success, Error>
        {
            public ErrorContainer(Error value)
            {
                this.Value = value;
            }

            public Error Value { get; set; }

            public override Either<NextValue, Error> Map<NextValue>(Func<Success, NextValue> mapper)
            {
                return this.Value;
            }

            public override void Do(Action<Success> successCallback, Action<Error> errorCallback)
            {
                errorCallback(this.Value);
            }
        }

        internal class SuccessContainer : Either<Success, Error>
        {
            public SuccessContainer(Success value)
            {
                this.Value = value;
            }

            public Success Value { get; set; }

            public override Either<NextValue, Error> Map<NextValue>(Func<Success, NextValue> mapper) =>
                mapper(this.Value);

            public override void Do(Action<Success> successCallback, Action<Error> errorCallback) =>
                successCallback(this.Value);
        }
        #endregion
    }
}
