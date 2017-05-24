using System;
using Xunit;
using Bearded.Monads;

namespace Bearded.Monads.Tests
{
    public class EitherTests
    {
        [Fact]
        public void CreateSuccessCase()
        {
            var expected = 42;

            var value = EitherSuccessOrError<int, string>.Create(expected);

            Assert.True(value.IsSuccess);
            Assert.False(value.IsError);

            Assert.Equal(expected, value.AsSuccess.Value);
        }

        [Fact]
        public void CreateErrorCase()
        {
            var errorMessage = "fail";

            var value = EitherSuccessOrError<int, string>.Create(errorMessage);

            Assert.True(value.IsError);
            Assert.False(value.IsSuccess);

            Assert.Equal(errorMessage, value.AsError.Value);
        }

        [Fact]
        public void MapIsApplied()
        {
            var givenValue = 42;
            var expectedValue = givenValue + 1;

            var value = EitherSuccessOrError<int, string>.Create(givenValue)
                .Map(i => i + 1);

            Assert.True(value.IsSuccess);
            Assert.Equal(expectedValue, value.AsSuccess.Value);
        }

        [Fact]
        public void MapIsANoopForError()
        {
            var error = "fail";

            var value = EitherSuccessOrError<int, string>.Create(error)
                .Map(i => i + 1);

            Assert.True(value.IsError);
            Assert.Equal(error, value.AsError.Value);
        }

        [Fact]
        public void ImplicitForSuccess()
        {
            var expectedValue = 1;
            Func<EitherSuccessOrError<int, string>> f;
            f = () => expectedValue;

            Assert.Equal(expectedValue, f().AsSuccess.Value);
        }

        [Fact]
        public void ImplicitForError()
        {
            var expectedValue = "fail";
            Func<EitherSuccessOrError<int, string>> f;
            f = () => expectedValue;

            Assert.Equal(expectedValue, f().AsError.Value);
        }

        [Fact]
        public void ImplicitForBoth()
        {
            var expectedSucess = 42;
            var expectedError = "fail";
            Func<Tuple<EitherSuccessOrError<int, string>, EitherSuccessOrError<int, string>>> f;
            f = () => new Tuple<EitherSuccessOrError<int, string>, EitherSuccessOrError<int, string>>(expectedSucess, expectedError);

            var success = f().Item1.AsSuccess;
            var error = f().Item2.AsError;

            Assert.Equal(expectedSucess, success.Value);
            Assert.Equal(expectedError, error.Value);
        }

        [Fact]
        public void AsOptionForSuccess_CarriesValue()
        {
            var expectedSucess = 42;

            EitherSuccessOrError<int, string> either = expectedSucess;

            bool wasCalled = false;
            var option = either.AsOption(e => { wasCalled = true; });

            Assert.False(wasCalled);
            Assert.True(option.IsSome);
            Assert.Equal(expectedSucess, option.ForceValue());
        }

        [Fact]
        public void AsOptionForError_IsNoneAndCallbackFired()
        {
            var expectedError = "fail";

            EitherSuccessOrError<int, string> either = expectedError;

            bool wasCalled = false;
            var option = either.AsOption(e => { wasCalled = true; });

            Assert.True(wasCalled);
            Assert.False(option.IsSome);
        }

        [Fact]
        public void SelectMany_BothSuccess()
        {
            var firstValue = 42;
            var secondValue = 666;

            var expectedValue = firstValue + secondValue;

            EitherSuccessOrError<int, string> firstEither = firstValue;
            EitherSuccessOrError<int, string> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(expectedValue, stuff.AsSuccess.Value);
        }

        [Fact]
        public void SelectMany_FirstError()
        {
            var firstError = "fail";
            var secondValue = 42;

            EitherSuccessOrError<int, string> firstEither = firstError;
            EitherSuccessOrError<int, string> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(firstError, stuff.AsError.Value);
        }

        [Fact]
        public void SelectMany_SecondError()
        {
            var firstValue = 42;
            var secondError = "fail";

            EitherSuccessOrError<int, string> firstEither = firstValue;
            EitherSuccessOrError<int, string> secondEither = secondError;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(secondError, stuff.AsError.Value);
        }

        [Fact]
        public void SelectMany_BothError_FirstError()
        {
            var firstValue = "epic";
            var secondValue = "fail";

            EitherSuccessOrError<int, string> firstEither = firstValue;
            EitherSuccessOrError<int, string> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(firstValue, stuff.AsError.Value);
        }

        #region Monad laws
        [Fact]
        public void LeftIdentity()
        {
            // forall a b, f :: (a -> Either a b). return a >>= f === f a
            // if we call selectmany with a function on a single value
            // the result should be the same as call the function directly 
            // without 

            var someName = "Wadler";
            EitherSuccessOrError<string, int> either = someName;

            Func<string, EitherSuccessOrError<string, int>> f;
            f = s => s.ToUpper();

            var viaOption = either.SelectMany(f);
            var straight = f(someName);

            Assert.Equal(straight, viaOption);
        }

        [Fact]
        public void RightIdentity()
        {
            // forall m. m >>= return === m
            // if we call selectmany with the return function
            // the result should be equal to the option we started with

            var someName = "Wadler";
            EitherSuccessOrError<string, int> option = someName;

            var viaOption = option.SelectMany(EitherSuccessOrError<string, int>.Create);

            Assert.Equal(option, viaOption);
        }

        [Fact]
        public void Associativity()
        {
            // Order of application of functions must associate

            var someName = "Wadler";
            EitherSuccessOrError<string, int> option = someName;

            Func<string, EitherSuccessOrError<string, int>> f;
            Func<string, EitherSuccessOrError<string, int>> g;

            f = s => s.ToUpper();
            g = s => s.ToLower();

            var first = option.SelectMany(f).SelectMany(g);
            var second = option.SelectMany(x => f(x).SelectMany(g));

            Assert.Equal(second, first);
        }
        #endregion
    }
}
