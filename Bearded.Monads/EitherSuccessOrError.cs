using System;
using System.Diagnostics;

namespace Bearded.Monads
{
    public abstract class EitherSuccessOrError<Success, Error> : IEquatable<EitherSuccessOrError<Success, Error>>
    {
        public ErrorContainer AsError { get { return this as ErrorContainer; } }
        public SuccessContainer AsSuccess { get { return this as SuccessContainer; } }

        public bool IsError { get { return this is ErrorContainer; } }
        public bool IsSuccess { get { return this is SuccessContainer; } }

        [DebuggerStepThrough]
        public static EitherSuccessOrError<Success, Error> Create(Error value)
        {
            return new ErrorContainer(value);
        }

        [DebuggerStepThrough]
        public static EitherSuccessOrError<Success, Error> Create(Success value)
        {
            return new SuccessContainer(value);
        }

        #region Abstract methods
        [DebuggerStepThrough]
        public abstract EitherSuccessOrError<NextSuccess,Error> Map<NextSuccess>(Func<Success,NextSuccess> mapper);

        [DebuggerStepThrough]
        public abstract void Do(Action<Success> successCallback, Action<Error> errorCallback);
        #endregion

        #region Operators
        [DebuggerStepThrough]
        public static implicit operator EitherSuccessOrError<Success, Error>(Error error)
        {
            return new ErrorContainer(error);
        }

        [DebuggerStepThrough]
        public static implicit operator EitherSuccessOrError<Success, Error>(Success success)
        {
            return new SuccessContainer(success);
        }
        #endregion

        public bool Equals(EitherSuccessOrError<Success, Error> other)
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
            return Equals((EitherSuccessOrError<Success, Error>) obj);
        }

        public override int GetHashCode()
        {
            if (this.IsSuccess)
                return this.AsSuccess.Value.GetHashCode();

            return this.AsError.Value.GetHashCode();
        }

        public static bool operator ==(EitherSuccessOrError<Success, Error> left, EitherSuccessOrError<Success, Error> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EitherSuccessOrError<Success, Error> left, EitherSuccessOrError<Success, Error> right)
        {
            return !Equals(left, right);
        }

        #region Implementations
        public class ErrorContainer : EitherSuccessOrError<Success, Error>
        {
            public ErrorContainer(Error value)
            {
                this.Value = value;
            }

            public Error Value { get; set; }

            public override EitherSuccessOrError<NextValue, Error> Map<NextValue>(Func<Success, NextValue> mapper)
            {
                return this.Value;
            }

            public override void Do(Action<Success> successCallback, Action<Error> errorCallback)
            {
                errorCallback(this.Value);
            }
        }

        public class SuccessContainer : EitherSuccessOrError<Success, Error>
        {
            public SuccessContainer(Success value)
            {
                this.Value = value;
            }

            public Success Value { get; set; }

            public override EitherSuccessOrError<NextValue, Error> Map<NextValue>(Func<Success, NextValue> mapper)
            {
                return mapper(this.Value);
            }

            public override void Do(Action<Success> successCallback, Action<Error> errorCallback)
            {
                successCallback(this.Value);
            }
        }
        #endregion
    }
}