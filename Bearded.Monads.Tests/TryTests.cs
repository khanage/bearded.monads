using System;
using System.Linq;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class TryTests
    {
        [Fact]
        public void CreateSuccessCase()
        {
            var expected = 42;

            var value = Try<int>.Create(expected);

            Assert.True(value.IsSuccess);
            Assert.False(value.IsError);

            Assert.Equal(expected, value.AsSuccess().Value);
        }

        [Fact]
        public void CreateErrorCase()
        {
            var errorMessage = new Exception("fail");

            var value = Try<int>.Create(errorMessage);

            Assert.True(value.IsError);
            Assert.False(value.IsSuccess);

            Assert.Equal(errorMessage, value.AsError().Value);
        }

        [Fact]
        public void MapIsApplied()
        {
            var givenValue = 42;
            var expectedValue = givenValue + 1;

            var value = Try<int>.Create(givenValue)
                .Map(i => i + 1);

            Assert.True(value.IsSuccess);
            Assert.Equal(expectedValue, value.AsSuccess().Value);
        }

        [Fact]
        public void MapIsANoopForError()
        {
            var error = new Exception("fail");

            var value = Try<int>.Create(error)
                .Map(i => i + 1);

            Assert.True(value.IsError);
            Assert.Equal(error, value.AsError().Value);
        }

        [Fact]
        public void ImplicitForSuccess()
        {
            var expectedValue = 1;
            Func<Try<int>> f;
            f = () => expectedValue;

            Assert.Equal(expectedValue, f().AsSuccess().Value);
        }

        [Fact]
        public void ImplicitForError()
        {
            var expectedValue = new Exception("fail");
            Func<Try<int>> f;
            f = () => expectedValue;

            Assert.Equal(expectedValue, f().AsError().Value);
        }

        [Fact]
        public void ImplicitForBoth()
        {
            var expectedSucess = 42;
            var expectedError = new Exception("fail");
            Func<Tuple<Try<int>, Try<int>>> f;
            f = () => new Tuple<Try<int>, Try<int>>(expectedSucess, expectedError);

            var success = f().Item1.AsSuccess();
            var error = f().Item2.AsError();

            Assert.Equal(expectedSucess, success.Value);
            Assert.Equal(expectedError, error.Value);
        }

        [Fact]
        public void AsOptionForSuccess_CarriesValue()
        {
            var expectedSucess = 42;

            Try<int> either = expectedSucess;

            bool wasCalled = false;
            var option = either.AsOption(e => { wasCalled = true; });

            Assert.False(wasCalled);
            Assert.True(option.IsSome);
            Assert.Equal(expectedSucess, option.ForceValue());
        }

        [Fact]
        public void AsOptionForError_IsNoneAndCallbackFired()
        {
            var expectedError = new Exception("fail");

            Try<int> either = expectedError;

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

            Try<int> firstEither = firstValue;
            Try<int> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(expectedValue, stuff.AsSuccess().Value);
        }

        [Fact]
        public void SelectMany_FirstError()
        {
            var firstError = new Exception("fail");
            var secondValue = 42;

            Try<int> firstEither = firstError;
            Try<int> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(firstError, stuff.AsError().Value);
        }

        [Fact]
        public void SelectMany_SecondError()
        {
            var firstValue = 42;
            var secondError = new Exception("fail");

            Try<int> firstEither = firstValue;
            Try<int> secondEither = secondError;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(secondError, stuff.AsError().Value);
        }

        [Fact]
        public void SelectMany_BothError_FirstError()
        {
            var firstValue = new Exception("epic");
            var secondValue = new Exception("fail");

            Try<int> firstEither = firstValue;
            Try<int> secondEither = secondValue;

            var stuff =
                from f in firstEither
                from s in secondEither
                select f + s;

            Assert.Equal(firstValue, stuff.AsError().Value);
        }

        [Fact]
        public void Traverse_Either_Ok()
        {
            var input = Enumerable.Range(1, 10);

            Try<int> isLessThan100(int i)
            {
                if (i < 100) return i;
                return new Exception("Failed");
            }

            var result = input.Traverse(isLessThan100);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Traverse_Either_NotOk()
        {
            var input = Enumerable.Range(100, 10);

            Try<int> isLessThan100(int i)
            {
                if (i < 100) return i;
                return new Exception("Failed");
            }

            var result = input.Traverse(isLessThan100);

            Assert.True(result.IsError);
        }

        [Fact]
        public void Sequence_OK()
        {
            var input = Enumerable.Range(1, 100).Select(Try<int>.Create);

            var result = input.Sequence();

            Assert.True(result.IsSuccess);
            Assert.Equal(100, result.Select(l => l.Count()).AsSuccess().Value);
        }

        [Fact]
        public void Sequence_NotOK()
        {
            var input = Enumerable.Range(1, 100).Select(Try<int>.Create).Append(Try<int>.Create(new Exception("Failed")));

            var result = input.Sequence();

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void WhereNot_Ok()
        {
            var expected = 10;
            var input = Try<int>.Create(expected);
            var result = input.WhereNot(i => i > 10, () => new Exception("Fail"));

            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.AsSuccess().Value);
        }

        [Fact]
        public void WhereNot_NotOk()
        {
            var input = Try<int>.Create(100);
            var expected = new Exception("Fail");
            var result = input.WhereNot(i => i > 10, () => expected);

            Assert.False(result.IsSuccess);
            Assert.Equal(expected, result.AsError().Value);
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
            Try<string> either = someName;

            Func<string, Try<string>> f;
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
            Try<string> option = someName;

            var viaOption = option.SelectMany(Try<string>.Create);

            Assert.Equal(option, viaOption);
        }

        [Fact]
        public void Associativity()
        {
            // Order of application of functions must associate

            var someName = "Wadler";
            Try<string> option = someName;

            Func<string, Try<string>> f;
            Func<string, Try<string>> g;

            f = s => s.ToUpper();
            g = s => s.ToLower();

            var first = option.SelectMany(f).SelectMany(g);
            var second = option.SelectMany(x => f(x).SelectMany(g));

            Assert.Equal(second, first);
        }
        #endregion
    }
}
