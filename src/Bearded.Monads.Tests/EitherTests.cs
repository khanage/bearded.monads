using System;
using NUnit.Framework;

namespace Bearded.Monads.Tests
{
    class EitherTests
    {
        [Test]
        public void CreateSuccessCase()
        {
            var expected = 42;

            var value = EitherSuccessOrError<int, string>.Create(expected);

            Assert.True(value.IsSuccess);
            Assert.False(value.IsError);

            Assert.That(value.AsSuccess.Value, Is.EqualTo(expected));
        }

        [Test]
        public void CreateErrorCase()
        {
            var errorMessage = "fail";

            var value = EitherSuccessOrError<int, string>.Create(errorMessage);

            Assert.True(value.IsError);
            Assert.False(value.IsSuccess);

            Assert.That(value.AsError.Value, Is.EqualTo(errorMessage));
        }

        [Test]
        public void MapIsApplied()
        {
            var givenValue = 42;
            var expectedValue = givenValue + 1;

            var value = EitherSuccessOrError<int, string>.Create(givenValue)
                .Map(i => i + 1);

            Assert.True(value.IsSuccess);
            Assert.That(value.AsSuccess.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void MapIsANoopForError()
        {
            var error = "fail";

            var value = EitherSuccessOrError<int, string>.Create(error)
                .Map(i => i + 1);

            Assert.True(value.IsError);
            Assert.That(value.AsError.Value, Is.EqualTo(error));
        }

        [Test]
        public void ImplicitForSuccess()
        {
            var expectedValue = 1;
            Func<EitherSuccessOrError<int, string>> f;
            f = () => expectedValue;

            Assert.That(f().AsSuccess.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ImplicitForError()
        {
            var expectedValue = "fail";
            Func<EitherSuccessOrError<int, string>> f;
            f = () => expectedValue;

            Assert.That(f().AsError.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ImplicitForBoth()
        {
            var expectedSucess = 42;
            var expectedError = "fail";
            Func<Tuple<EitherSuccessOrError<int, string>, EitherSuccessOrError<int, string>>> f;
            f = () => new Tuple<EitherSuccessOrError<int, string>, EitherSuccessOrError<int, string>>(expectedSucess,expectedError);

            var success = f().Item1.AsSuccess;
            var error = f().Item2.AsError;

            Assert.That(success.Value, Is.EqualTo(expectedSucess));
            Assert.That(error.Value, Is.EqualTo(expectedError));
        }

        [Test]
        public void AsOptionForSuccess_CarriesValue()
        {
            var expectedSucess = 42;

            EitherSuccessOrError<int, string> either = expectedSucess;

            bool wasCalled = false;
            var option = either.AsOption(e => { wasCalled = true; });

            Assert.False(wasCalled);
            Assert.True(option.IsSome);
            Assert.That(option.ForceValue(), Is.EqualTo(expectedSucess));
        }

        [Test]
        public void AsOptionForError_IsNoneAndCallbackFired()
        {
            var expectedError = "fail";

            EitherSuccessOrError<int, string> either = expectedError;

            bool wasCalled = false;
            var option = either.AsOption(e => { wasCalled = true; });

            Assert.True(wasCalled);
            Assert.False(option.IsSome);
        }

        [Test]
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

            Assert.That(stuff.AsSuccess.Value, Is.EqualTo(expectedValue));
        }

        [Test]
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

            Assert.That(stuff.AsError.Value, Is.EqualTo(firstError));
        }

        [Test]
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

            Assert.That(stuff.AsError.Value, Is.EqualTo(secondError));
        }

        [Test]
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

            Assert.That(stuff.AsError.Value, Is.EqualTo(firstValue));
        }

        #region Monad laws
        [Test]
        public void LeftIdentity()
        {
            // forall a b, f :: (a -> Either a b). return a >>= f === f a
            // if we call selectmany with a function on a single value
            // the result should be the same as call the function directly 
            // without 

            var someName = "Wadler";
            EitherSuccessOrError<string,int> either = someName;

            Func<string, EitherSuccessOrError<string, int>> f;
            f = s => s.ToUpper();

            var viaOption = either.SelectMany(f);
            var straight = f(someName);

            Assert.That(viaOption, Is.EqualTo(straight));
        }

        [Test]
        public void RightIdentity()
        {
            // forall m. m >>= return === m
            // if we call selectmany with the return function
            // the result should be equal to the option we started with

            var someName = "Wadler";
            EitherSuccessOrError<string, int> option = someName;

            var viaOption = option.SelectMany(EitherSuccessOrError<string, int>.Create);

            Assert.That(viaOption, Is.EqualTo(option));
        }

        [Test]
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

            Assert.That(first, Is.EqualTo(second));
        }
        #endregion
    }
}
