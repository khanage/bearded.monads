using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bearded.Monads;
using Xunit;
using Xunit.Sdk;

namespace Bearded.Monads.Tests
{
    public class OptionTests
    {
        [Fact]
        public void Return()
        {
            var someInteger = 42;
            var option = Option.Return(someInteger);

            Assert.True(option.IsSome);
            Assert.Equal(42, option.ForceValue());
        }

        [Fact]
        public void None()
        {
            var option = Option<int>.None;

            Assert.False(option.IsSome);
            Assert.Throws<InvalidOperationException>(delegate { var _ = option.ForceValue(); });
        }

        [Fact]
        public void SomeEquality()
        {
            var elem = 42;
            var firstOption = Option.Return(elem);
            var secondOption = Option.Return(42);
            var differentValuedOption = Option.Return(666);
            var differentTypedOption = Option.Return("hello there");
            var sameTypedNone = Option<int>.None;
            var differentTypedNone = Option<string>.None;

            Assert.Equal(secondOption, firstOption);
            Assert.NotEqual(differentValuedOption, firstOption);
            Assert.False(differentTypedOption.Equals(firstOption));
            Assert.False(sameTypedNone.Equals(firstOption));
            Assert.False(differentTypedNone.Equals(firstOption));
        }

        [Fact]
        public void NoneEquality()
        {
            var none = Option<int>.None;
            var sameTypedNone = Option<int>.None;
            var sameTypeSome = Option<int>.Return(42);
            var differentTypeSome = Option.Return("Hello, world");

            Assert.Equal(sameTypedNone, none);
            Assert.NotEqual(sameTypeSome, none);
            Assert.False(differentTypeSome == none);
        }

        [Fact]
        public void MappedNoneEquality()
        {
            var left = Option<int>.None;
            var right = Option<string>.None.Map(_ => 1);

            Assert.Equal(right, left);
        }

        [Fact]
        public void MapNoneIsNoop()
        {
            var none = Option<int>.None;

            var result = none.Map(i => i + 1);

            Assert.Equal(none, result);
        }

        [Fact]
        public void MapSomeAffectsValue()
        {
            var option = Option.Return(41);

            var resultOption = option.Map(i => i + 1);

            Assert.True(resultOption.IsSome);
            Assert.Equal(42, resultOption.ForceValue());
        }

        [Fact]
        public void DoIsCalledForSome()
        {
            var option = Option.Return(42);

            var i = 1;

            option.Do(j => i = j);

            Assert.Equal(42, i);
        }

        [Fact]
        public void DoWithOverloadIsCalledForSome()
        {
            var option = Option.Return(42);

            var i = 1;

            option.Do(j => i = j, () => { });

            Assert.Equal(42, i);
        }

        [Fact]
        public void DoWithOverloadIsCalledForNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.Do(j => { }, () => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void ElseWithDefault()
        {
            var option = Option<int>.None;
            var def = option.ElseDefault();

            Assert.Equal(default(int), def);
        }

        [Fact]
        public void ElseWithSomeReturnsSome()
        {
            var option = Option.Return(42);
            var def = option.Else(() => 666);

            Assert.Equal(42, def);
        }

        [Fact]
        public void ElseWithNoneReturnsElse()
        {
            var option = Option<int>.None;
            var def = option.Else(() => 666);

            Assert.Equal(666, def);
        }

        [Fact]
        public void SelectManyFlattensOption()
        {
            var optionOption = Option.Return(Option.Return(42));
            var option = optionOption.SelectMany(x => x);

            Assert.Equal(42, option.ForceValue());
        }

        [Fact]
        public void ConcatLeftOnly()
        {
            var left = Option.Return(42);
            var right = Option<int>.None;

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Fact]
        public void ConcatRightOnly()
        {
            var left = Option<int>.None;
            var right = Option.Return(42);

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Fact]
        public void ConcatBoth()
        {
            var left = Option.Return(42);
            var right = Option.Return(42);

            var option = left.Concat(right);

            Assert.Equal(new Tuple<int, int>(42, 42), option.ForceValue());
        }

        [Fact]
        public void WhereWithMatchingPredicateReturnsSome()
        {
            var option = Option.Return(42);

            var result = option.Where(i => true);

            Assert.Equal(option, result);
        }

        [Fact]
        public void WhereWithFailingPredicateReturnsNone()
        {
            var option = Option.Return(42);

            var result = option.Where(i => false);

            Assert.NotEqual(option, result);
            Assert.Equal(Option<int>.None, result);
        }

        [Fact]
        public void EmptyCallbackForSome()
        {
            var option = Option.Return(42);

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Fact]
        public void EmptyCallbackForNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void CallbackForWhenSomeOnSome()
        {
            var option = Option.Return(42);

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.Equal(option.ForceValue(), i);
        }

        [Fact]
        public void CallbackForWhenSomeOnNone()
        {
            var option = Option<int>.None;

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.Equal(666, i);
        }

        [Fact]
        public void CallbackForWhenNoneOnSome()
        {
            var option = Option.Return(42);

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Fact]
        public void CallbackForWhenNoneOnNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void TrueOperatorOnSome()
        {
            if (Option.Return(42)) { }
            else
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void TrueOperatorOnNone()
        {
            if (Option<int>.None)
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void SelectManyBothSome()
        {
            var result =
                from a in Option.Return(21)
                from b in Option.Return(21)
                select a + b;

            Assert.Equal(42, result.ForceValue());
        }

        [Fact]
        public void SelectManyFirstNone()
        {
            var result =
                from a in Option<int>.None
                from b in Option.Return(21)
                select a + b;

            Assert.False(result.IsSome);
        }

        [Fact]
        public void SelectManySecondNone()
        {
            var result =
                from a in Option.Return(21)
                from b in Option<int>.None
                select a + b;

            Assert.False(result.IsSome);
        }

        [Fact]
        public void FirstOrDefaultEmptyList()
        {
            var items = new List<Option<int>>();
            var result = items.FirstOrDefault();

            Assert.False(result.IsSome);
        }

        [Fact]
        public void FirstOrDefaultFirstSome()
        {
            var items = new List<Option<int>> { Option.Return(42), Option.Return(666) };
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.Equal(42, result.ForceValue());
        }

        [Fact]
        public void FirstOrDefaultSecondSome()
        {
            var items = new List<Option<int>> { Option<int>.None, Option.Return(666) };
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.Equal(666, result.ForceValue());
        }

        [Fact]
        public void FirstOrDefaultAllNone()
        {
            var items = new List<Option<int>> { Option<int>.None, Option<int>.None };
            var result = items.FirstOrDefault();

            Assert.False(result.IsSome);
        }

        [Fact]
        public void ThenWithTrueValue()
        {
            var option = true.Then(() => 42);

            Assert.True(option.IsSome);
            Assert.Equal(42, option.ForceValue());
        }

        [Fact]
        public void ThenWithFalseValue()
        {
            var option = false.Then(() => 42);

            Assert.False(option.IsSome);
        }

        [Fact]
        public void NoneIfNullNonNull()
        {
            var str = "hello, world";

            var option = str.NoneIfNull();

            Assert.True(option.IsSome);
            Assert.Equal(str, option.ForceValue());
        }

        [Fact]
        public void NoneIfNullNull()
        {
            string str = null;

            var option = str.NoneIfNull();

            Assert.False(option.IsSome);
        }

        [Fact]
        public void NoneIfFalseOnFalse()
        {
            var option = false.NoneIfFalse();

            Assert.False(option.IsSome);
        }

        [Fact]
        public void NoneIfFalseOnTrue()
        {
            var option = true.NoneIfFalse();

            Assert.True(option.IsSome);
        }

        [Fact]
        public void TryGetValueForPresentKey()
        {
            var key = "hello";
            var expectedValue = 42;

            var dict = new Dictionary<string, object> { { key, expectedValue } };

            var result = dict.MaybeGetValue(key);

            Assert.True(result.IsSome);
            Assert.Equal(expectedValue, result.ForceValue());
        }

        [Fact]
        public void MaybeCast_OK()
        {
            var downcast = (object)4;

            var result = downcast.MaybeCast<int>();

            Assert.True(result.IsSome);
            Assert.Equal(4, result.ForceValue());
        }

        [Fact]
        public void TryGetValueForMissingKey()
        {
            var key = "hello";
            var expectedValue = 42;

            var dict = new Dictionary<string, object> { { "irrelevent", expectedValue } };

            var result = dict.MaybeGetValue(key);

            Assert.True(!result.IsSome);
        }
        [Fact]
        public void TryGetValuesForPresentKey()
        {
            var key = "hello";
            var expectedValues = new[] { 11, 42 };

            var lookup = new[] { Tuple.Create("hello", 11), Tuple.Create("hello", 42) }
                .ToLookup(t => t.Item1, t => t.Item2);

            var result = lookup.MaybeGetValues(key);

            Assert.True(result.IsSome);
            Assert.Equal(expectedValues, result.ForceValue());
        }

        [Fact]
        public void TryGetValuesForMissingKey()
        {
            var key = "hello";

            var lookup = new[] { Tuple.Create("irrelevant", 11), Tuple.Create("irrelevant", 42) }
               .ToLookup(t => t.Item1, t => t.Item2);

            var result = lookup.MaybeGetValues(key);

            Assert.False(result.IsSome);
        }

        [Fact]
        public void AggregateWithExistingMembers()
        {
            var list = new[] { 1, 2, 3 };

            var result = list.AggregateOrNone((total, current) => total + current);

            Assert.Equal(6, result.ForceValue());
        }

        [Fact]
        public void AggregateWithMissingMembers()
        {
            var list = Enumerable.Empty<int>();

            var result = list.AggregateOrNone((total, current) => total + current);

            Assert.True(!result.IsSome);
        }

        [Fact]
        public void AggregateWithOptions()
        {
            var list = new[] { 1.AsOption(), 2.AsOption(), 3.AsOption() };

            var result = list.AggregateOrNone((total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.Equal(6, result.ForceValue());
        }

        [Fact]
        public void AggregateWithOptionsAndNone()
        {
            var list = new[] { 1.AsOption(), Option<int>.None, 3.AsOption() };

            var result = list.AggregateOrNone((total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.True(!result.IsSome);
        }

        [Fact]
        public void AggregateWithExistingMembersAndSeed()
        {
            var list = new[] { 1, 2, 3 };

            var result = list.AggregateOrNone(10, (total, current) => total + current);

            Assert.Equal(16, result.ForceValue());
        }

        [Fact]
        public void AggregateWithMissingMembersAndSeed()
        {
            var list = Enumerable.Empty<int>();

            var result = list.AggregateOrNone(10, (total, current) => total + current);

            Assert.True(!result.IsSome);
        }
        [Fact]
        public void AggregateWithOptionsAndSeed()
        {
            var list = new[] { 1.AsOption(), 2.AsOption(), 3.AsOption() };

            var result = list.AggregateOrNone(10.AsOption(), (total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.Equal(16, result.ForceValue());
        }

        [Fact]
        public void AggregateWithOptionsAndNoneAndSeed()
        {
            var list = new[] { 1.AsOption(), Option<int>.None, 3.AsOption() };

            var result = list.AggregateOrNone(10.AsOption(), (total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.True(!result.IsSome);
        }
        [Fact]
        public void ImplicitCastBool()
        {
            bool b = "IsSome".AsOption();
            Assert.True(b);
            b = Option<string>.None;
            Assert.False(b);
        }

        [Fact]
        public void ImplicitCastNull()
        {
            string n = null;
            Option<string> none = n;

            Assert.False(none);
        }

        [Theory]
        [InlineData(1, 2, 1)]
        [InlineData(1, null, 1)]
        [InlineData(null, 2, 2)]
        [InlineData(null, null, null)]
        public void PipeOperator(int? x, int? y, int? expected)
        {
            Assert.Equal(x.NoneIfEmpty() | y.NoneIfEmpty(), expected.NoneIfEmpty());
        }

        [Fact]
        public void PipeOperatorShortCircuit()
        {
            var some = new object().AsOption();
            var none = Option<object>.None;
            Func<Option<object>> fail = () => { throw new Exception(); };

            // Does not throw doesn't have
            // an explicit operator
            var result = some | fail;
            Assert.Throws<Exception>(() => { var bad = none | fail; });
        }

        [Fact]
        public async Task DoAsyncWithSome()
        {
            var some = new object().AsOption();
            var wasCalled = false;

            await some.DoAsync(async x =>
            {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.True(wasCalled);
        }

        [Fact]
        public async Task DoAsyncWithNone()
        {
            var none = Option<object>.None;
            var wasCalled = false;

            await none.DoAsync(async x =>
            {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.False(wasCalled);
        }

        [Fact]
        public async Task DoAsyncWithNull()
        {
            Option<object> none = null;
            var wasCalled = false;

            await none.DoAsync(async x =>
            {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.False(wasCalled);
        }

        [Fact]
        public async Task MapAsyncWithSome()
        {
            var some = 0.AsOption();

            var result = await some.MapAsync(
                async x => await Task.FromResult(x + 1));

            Assert.Equal(1.AsOption(), result);
        }

        [Fact]
        public async Task MapAsyncWithNone()
        {
            var none = Option<int>.None;

            var result = await none.MapAsync(
                async x => await Task.FromResult(x + 1));

            Assert.Equal(Option<int>.None, result);
        }

        [Fact]
        public async Task MapAsyncWithNull()
        {
            Option<object> none = null;

            var result = await none.MapAsync(
                async x => await Task.FromResult(0));

            Assert.Equal(Option<int>.None, result);
        }

        [Fact]
        public void Traverse_Ok()
        {
            var input = Enumerable.Range(1, 10);

            Option<int> isLessThan100(int i) => Option<int>.Return(i).Where(n => n < 100);

            var result = input.Traverse(isLessThan100);

            Assert.True(result.IsSome);
        }

        [Fact]
        public void Traverse_NotOk()
        {
            var input = Enumerable.Range(100, 10);

            Option<int> isLessThan100(int i) => Option<int>.Return(i).Where(n => n < 100);

            var result = input.Traverse(isLessThan100);

            Assert.False(result.IsSome);
        }

        [Fact]
        public void EmptyNullable_ElseDefault_Null()
        {
            Option<long?> thing = Option<long?>.None;

            Assert.Null(thing.ElseDefault());
        }

        [Fact]
        public void Sequence_Ok()
        {
            var input = Enumerable.Range(1, 10).Select(Option.Return);

            var result = input.Sequence();

            Assert.True(result.IsSome);
            result.Do(l => Assert.Equal(10, l.Count()));
        }

        [Fact]
        public void Sequence_NotOk()
        {
            var input = Enumerable.Range(1, 10).Select(Option.Return).Append(Option<int>.None);

            var result = input.Sequence();

            Assert.False(result.IsSome);
        }

        #region Monad laws
        [Fact]
        public void LeftIdentity()
        {
            // forall a, f :: (a -> Option a). return a >>= f === f a
            // if we call selectmany with a function on a single value
            // the result should be the same as call the function directly
            // without

            var someName = "Wadler";
            var option = Option.Return(someName);

            Func<string, Option<string>> f;
            f = s => s.ToUpper();

            var viaOption = option.SelectMany(f);
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
            var option = Option.Return(someName);

            var viaOption = option.SelectMany(Option.Return);

            Assert.Equal(option, viaOption);
        }

        [Fact]
        public void Associativity()
        {
            // Order of application of functions must associate

            var someName = "Wadler";
            var option = Option.Return(someName);

            Func<string, Option<string>> f;
            Func<string, Option<string>> g;

            f = s => s.ToUpper();
            g = s => s.ToLower();

            var first = option.SelectMany(f).SelectMany(g);
            var second = option.SelectMany(x => f(x).SelectMany(g));

            Assert.Equal(second, first);
        }
        #endregion
    }
}
