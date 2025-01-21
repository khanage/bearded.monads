using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bearded.Monads;
using Xunit;

#pragma warning disable CS0618
namespace Bearded.Monads.Tests
{
    public class OptionUnsafeTests
    {
        [Fact]
        public void Return()
        {
            var someInteger = 42;
            var option = OptionUnsafe.Return(someInteger);

            Assert.True(option.IsSome);
            Assert.Equal(42, option.ForceValue());
        }

        [Fact]
        public void NullIsOk()
        {
            String input = null;
            var option = OptionUnsafe.Return(input);

            Assert.True(option.IsSome);
            Assert.Null(option.ForceValue());
        }

        [Fact]
        public void None()
        {
            var option = OptionUnsafe<int>.None;

            Assert.False(option.IsSome);
            Assert.Throws<InvalidOperationException>(
                delegate
                {
                    var _ = option.ForceValue();
                }
            );
        }

        [Fact]
        public void SomeEquality()
        {
            var elem = 42;
            var firstOptionUnsafe = OptionUnsafe.Return(elem);
            var secondOptionUnsafe = OptionUnsafe.Return(42);
            var differentValuedOptionUnsafe = OptionUnsafe.Return(666);
            var differentTypedOptionUnsafe = OptionUnsafe.Return("hello there");
            var sameTypedNone = OptionUnsafe<int>.None;
            var differentTypedNone = OptionUnsafe<string>.None;

            Assert.Equal(secondOptionUnsafe, firstOptionUnsafe);
            Assert.NotEqual(differentValuedOptionUnsafe, firstOptionUnsafe);
            Assert.False(differentTypedOptionUnsafe.Equals(firstOptionUnsafe));
            Assert.False(sameTypedNone.Equals(firstOptionUnsafe));
            Assert.False(differentTypedNone.Equals(firstOptionUnsafe));
        }

        [Fact]
        public void NoneEquality()
        {
            var none = OptionUnsafe<int>.None;
            var sameTypedNone = OptionUnsafe<int>.None;
            var sameTypeSome = OptionUnsafe<int>.Return(42);
            var differentTypeSome = OptionUnsafe.Return("Hello, world");

            Assert.Equal(sameTypedNone, none);
            Assert.NotEqual(sameTypeSome, none);
            Assert.False(differentTypeSome == none);
        }

        [Fact]
        public void MappedNoneEquality()
        {
            var left = OptionUnsafe<int>.None;
            var right = OptionUnsafe<string>.None.Map(_ => 1);

            Assert.Equal(right, left);
        }

        [Fact]
        public void MapNoneIsNoop()
        {
            var none = OptionUnsafe<int>.None;

            var result = none.Map(i => i + 1);

            Assert.Equal(none, result);
        }

        [Fact]
        public void MapSomeAffectsValue()
        {
            var option = OptionUnsafe.Return(41);

            var resultOptionUnsafe = option.Map(i => i + 1);

            Assert.True(resultOptionUnsafe.IsSome);
            Assert.Equal(42, resultOptionUnsafe.ForceValue());
        }

        [Fact]
        public void DoIsCalledForSome()
        {
            var option = OptionUnsafe.Return(42);

            var i = 1;

            option.Do(j => i = j);

            Assert.Equal(42, i);
        }

        [Fact]
        public void DoWithOverloadIsCalledForSome()
        {
            var option = OptionUnsafe.Return(42);

            var i = 1;

            option.Do(j => i = j, () => { });

            Assert.Equal(42, i);
        }

        [Fact]
        public void DoWithOverloadIsCalledForNone()
        {
            var option = OptionUnsafe<int>.None;

            var wasCalled = false;

            option.Do(j => { }, () => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void ElseWithDefault()
        {
            var option = OptionUnsafe<int>.None;
            var def = option.ElseDefault();

            Assert.Equal(default(int), def);
        }

        [Fact]
        public void ElseWithSomeReturnsSome()
        {
            var option = OptionUnsafe.Return(42);
            var def = option.Else(() => 666);

            Assert.Equal(42, def);
        }

        [Fact]
        public void ElseWithNoneReturnsElse()
        {
            var option = OptionUnsafe<int>.None;
            var def = option.Else(() => 666);

            Assert.Equal(666, def);
        }

        [Fact]
        public void SelectManyFlattensOptionUnsafe()
        {
            var optionOptionUnsafe = OptionUnsafe.Return(OptionUnsafe.Return(42));
            var option = optionOptionUnsafe.SelectMany(x => x);

            Assert.Equal(42, option.ForceValue());
        }

        [Fact]
        public void ConcatLeftOnly()
        {
            var left = OptionUnsafe.Return(42);
            var right = OptionUnsafe<int>.None;

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Fact]
        public void ConcatRightOnly()
        {
            var left = OptionUnsafe<int>.None;
            var right = OptionUnsafe.Return(42);

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Fact]
        public void ConcatBoth()
        {
            var left = OptionUnsafe.Return(42);
            var right = OptionUnsafe.Return(42);

            var option = left.Concat(right);

            Assert.Equal(new Tuple<int, int>(42, 42), option.ForceValue());
        }

        [Fact]
        public void WhereWithMatchingPredicateReturnsSome()
        {
            var option = OptionUnsafe.Return(42);

            var result = option.Where(i => true);

            Assert.Equal(option, result);
        }

        [Fact]
        public void WhereWithFailingPredicateReturnsNone()
        {
            var option = OptionUnsafe.Return(42);

            var result = option.Where(i => false);

            Assert.NotEqual(option, result);
            Assert.Equal(OptionUnsafe<int>.None, result);
        }

        [Fact]
        public void EmptyCallbackForSome()
        {
            var option = OptionUnsafe.Return(42);

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Fact]
        public void EmptyCallbackForNone()
        {
            var option = OptionUnsafe<int>.None;

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void CallbackForWhenSomeOnSome()
        {
            var option = OptionUnsafe.Return(42);

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.Equal(option.ForceValue(), i);
        }

        [Fact]
        public void CallbackForWhenSomeOnNone()
        {
            var option = OptionUnsafe<int>.None;

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.Equal(666, i);
        }

        [Fact]
        public void CallbackForWhenNoneOnSome()
        {
            var option = OptionUnsafe.Return(42);

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Fact]
        public void CallbackForWhenNoneOnNone()
        {
            var option = OptionUnsafe<int>.None;

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Fact]
        public void TrueOperatorOnSome()
        {
            if (OptionUnsafe.Return(42)) { }
            else
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void TrueOperatorOnNone()
        {
            if (OptionUnsafe<int>.None)
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void SelectManyBothSome()
        {
            var result =
                from a in OptionUnsafe.Return(21)
                from b in OptionUnsafe.Return(21)
                select a + b;

            Assert.Equal(42, result.ForceValue());
        }

        [Fact]
        public void SelectManyFirstNone()
        {
            var result =
                from a in OptionUnsafe<int>.None
                from b in OptionUnsafe.Return(21)
                select a + b;

            Assert.False(result.IsSome);
        }

        [Fact]
        public void SelectManySecondNone()
        {
            var result =
                from a in OptionUnsafe.Return(21)
                from b in OptionUnsafe<int>.None
                select a + b;

            Assert.False(result.IsSome);
        }

        [Fact]
        public void FirstOrDefaultEmptyList()
        {
            var items = new List<OptionUnsafe<int>>();
            var result = items.FirstOrDefault();

            Assert.False(result.IsSome);
        }

        [Fact]
        public void FirstOrDefaultFirstSome()
        {
            var items = new List<OptionUnsafe<int>>
            {
                OptionUnsafe.Return(42),
                OptionUnsafe.Return(666),
            };
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.Equal(42, result.ForceValue());
        }

        [Fact]
        public void FirstOrDefaultSecondSome()
        {
            var items = new List<OptionUnsafe<int>>
            {
                OptionUnsafe<int>.None,
                OptionUnsafe.Return(666),
            };
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.Equal(666, result.ForceValue());
        }

        [Fact]
        public void FirstOrDefaultAllNone()
        {
            var items = new List<OptionUnsafe<int>>
            {
                OptionUnsafe<int>.None,
                OptionUnsafe<int>.None,
            };
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

            var result = dict.MaybeGetValueUnsafe(key);

            Assert.True(result.IsSome);
            Assert.Equal(expectedValue, result.ForceValue());
        }

        [Fact]
        public void TryGetValueForMissingKey()
        {
            var key = "hello";
            var expectedValue = 42;

            var dict = new Dictionary<string, object> { { "irrelevent", expectedValue } };

            var result = dict.MaybeGetValueUnsafe(key);

            Assert.True(!result.IsSome);
        }

        [Fact]
        public void TryGetValuesForPresentKey()
        {
            var key = "hello";
            var expectedValues = new[] { 11, 42 };

            var lookup = new[] { Tuple.Create("hello", 11), Tuple.Create("hello", 42) }.ToLookup(
                t => t.Item1,
                t => t.Item2
            );

            var result = lookup.MaybeGetValuesUnsafe(key);

            Assert.True(result.IsSome);
            Assert.Equal(expectedValues, result.ForceValue());
        }

        [Fact]
        public void TryGetValuesForMissingKey()
        {
            var key = "hello";

            var lookup = new[]
            {
                Tuple.Create("irrelevant", 11),
                Tuple.Create("irrelevant", 42),
            }.ToLookup(t => t.Item1, t => t.Item2);

            var result = lookup.MaybeGetValuesUnsafe(key);

            Assert.False(result.IsSome);
        }

        [Fact]
        public void ImplicitCastBool()
        {
            bool b = "IsSome".AsOptionUnsafe();
            Assert.True(b);
            b = OptionUnsafe<string>.None;
            Assert.False(b);
        }

        [Fact]
        public void ImplicitCastNull()
        {
            string n = null;
            OptionUnsafe<string> none = n;

            Assert.True(none);
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
            var some = new object().AsOptionUnsafe();
            var none = OptionUnsafe<object>.None;
            Func<OptionUnsafe<object>> fail = () =>
            {
                throw new Exception();
            };

            // Does not throw doesn't have
            // an explicit operator
            var result = some | fail;
            Assert.Throws<Exception>(() =>
            {
                var bad = none | fail;
            });
        }

        [Fact]
        public async Task DoAsyncWithSome()
        {
            var some = new object().AsOptionUnsafe();
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
            var none = OptionUnsafe<object>.None;
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
            OptionUnsafe<object> none = null;
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
            var some = 0.AsOptionUnsafe();

            var result = await some.MapAsync(async x => await Task.FromResult(x + 1));

            Assert.Equal(1.AsOptionUnsafe(), result);
        }

        [Fact]
        public async Task MapAsyncWithNone()
        {
            var none = OptionUnsafe<int>.None;

            var result = await none.MapAsync(async x => await Task.FromResult(x + 1));

            Assert.Equal(OptionUnsafe<int>.None, result);
        }

        [Fact]
        public async Task MapAsyncWithNull()
        {
            OptionUnsafe<object> none = null;

            var result = await none.MapAsync(async x => await Task.FromResult(0));

            Assert.Equal(OptionUnsafe<int>.None, result);
        }

        [Fact]
        public void Traverse_Ok()
        {
            var input = Enumerable.Range(1, 10);

            OptionUnsafe<int> isLessThan100(int i) =>
                OptionUnsafe<int>.Return(i).Where(n => n < 100);

            var result = input.Traverse(isLessThan100);

            Assert.True(result.IsSome);
        }

        [Fact]
        public void Traverse_NotOk()
        {
            var input = Enumerable.Range(100, 10);

            OptionUnsafe<int> isLessThan100(int i) =>
                OptionUnsafe<int>.Return(i).Where(n => n < 100);

            var result = input.Traverse(isLessThan100);

            Assert.False(result.IsSome);
        }

        [Fact]
        public void Sequence_Ok()
        {
            var input = Enumerable.Range(1, 10).Select(OptionUnsafe.Return);

            var result = input.Sequence();

            Assert.True(result.IsSome);
            result.Do(l => Assert.Equal(10, l.Count()));
        }

        [Fact]
        public void Sequence_NotOk()
        {
            var input = Enumerable
                .Range(1, 10)
                .Select(OptionUnsafe.Return)
                .Append(OptionUnsafe<int>.None);

            var result = input.Sequence();

            Assert.False(result.IsSome);
        }

        #region Monad laws
        [Fact]
        public void LeftIdentity()
        {
            // forall a, f :: (a -> OptionUnsafe a). return a >>= f === f a
            // if we call selectmany with a function on a single value
            // the result should be the same as call the function directly
            // without

            var someName = "Wadler";
            var option = OptionUnsafe.Return(someName);

            Func<string, OptionUnsafe<string>> f;
            f = s => s.ToUpper();

            var viaOptionUnsafe = option.SelectMany(f);
            var straight = f(someName);

            Assert.Equal(straight, viaOptionUnsafe);
        }

        [Fact]
        public void RightIdentity()
        {
            // forall m. m >>= return === m
            // if we call selectmany with the return function
            // the result should be equal to the option we started with

            var someName = "Wadler";
            var option = OptionUnsafe.Return(someName);

            var viaOptionUnsafe = option.SelectMany(OptionUnsafe.Return);

            Assert.Equal(option, viaOptionUnsafe);
        }

        [Fact]
        public void Associativity()
        {
            // Order of application of functions must associate

            var someName = "Wadler";
            var option = OptionUnsafe.Return(someName);

            Func<string, OptionUnsafe<string>> f;
            Func<string, OptionUnsafe<string>> g;

            f = s => s.ToUpper();
            g = s => s.ToLower();

            var first = option.SelectMany(f).SelectMany(g);
            var second = option.SelectMany(x => f(x).SelectMany(g));

            Assert.Equal(second, first);
        }
        #endregion
    }
}
