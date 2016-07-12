using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Bearded.Monads.Tests
{
    class OptionTests
    {
        [Test]
        public void Return()
        {
            var someInteger = 42;

            var option = Option.Return(someInteger);

            Assert.That(option.IsSome, Is.True);
            Assert.That(option.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void None()
        {
            var option = Option<int>.None;

            Assert.That(option.IsSome, Is.False);
            Assert.Throws<InvalidOperationException>(delegate { var _ = option.ForceValue(); });
        }

        [Test]
        public void SomeEquality()
        {
            var elem = 42;
            var firstOption = Option.Return(elem);
            var secondOption = Option.Return(42);
            var differentValuedOption = Option.Return(666);
            var differentTypedOption = Option.Return("hello there");
            var sameTypedNone = Option<int>.None;
            var differentTypedNone = Option<string>.None;

            Assert.That(firstOption, Is.EqualTo(secondOption));
            Assert.That(firstOption, Is.Not.EqualTo(differentValuedOption));
            Assert.That(firstOption, Is.Not.EqualTo(differentTypedOption));
            Assert.That(firstOption, Is.Not.EqualTo(sameTypedNone));
            Assert.That(firstOption, Is.Not.EqualTo(differentTypedNone));
        }

        [Test]
        public void NoneEquality()
        {
            var none = Option<int>.None;
            var sameTypedNone = Option<int>.None;
            var sameTypeSome = Option<int>.Return(42);
            var differentTypeSome = Option.Return("Hello, world");

            Assert.That(none, Is.EqualTo(sameTypedNone));
            Assert.That(none, Is.Not.EqualTo(sameTypeSome));
            Assert.That(none, Is.Not.EqualTo(differentTypeSome));
        }

        [Test]
        public void MappedNoneEquality()
        {
            var left = Option<int>.None;
            var right = Option<string>.None.Map(_ => 1);

            Assert.That(left, Is.EqualTo(right));
        }

        [Test]
        public void MapNoneIsNoop()
        {
            var none = Option<int>.None;

            var result = none.Map(i => i + 1);

            Assert.That(result, Is.EqualTo(none));
        }

        [Test]
        public void MapSomeAffectsValue()
        {
            var option = Option.Return(41);

            var resultOption = option.Map(i => i + 1);

            Assert.True(resultOption.IsSome);
            Assert.That(resultOption.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void DoIsCalledForSome()
        {
            var option = Option.Return(42);

            var i = 1;

            option.Do(j => i = j);

            Assert.That(i, Is.EqualTo(42));
        }

        [Test]
        public void DoWithOverloadIsCalledForSome()
        {
            var option = Option.Return(42);

            var i = 1;

            option.Do(j => i = j, () => { });

            Assert.That(i, Is.EqualTo(42));
        }

        [Test]
        public void DoWithOverloadIsCalledForNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.Do(j => { }, () => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Test]
        public void ElseWithDefault()
        {
            var option = Option<int>.None;
            var def = option.ElseDefault();
            
            Assert.That(def, Is.EqualTo(default(int)));
        }

        [Test]
        public void ElseWithSomeReturnsSome()
        {
            var option = Option.Return(42);
            var def = option.Else(() => 666);

            Assert.That(def, Is.EqualTo(42));
        }

        [Test]
        public void ElseWithNoneReturnsElse()
        {
            var option = Option<int>.None;
            var def = option.Else(() => 666);

            Assert.That(def, Is.EqualTo(666));
        }

        [Test]
        public void SelectManyFlattensOption()
        {
            var optionOption = Option.Return(Option.Return(42));
            var option = optionOption.SelectMany(x => x);

            Assert.That(option.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void ConcatLeftOnly()
        {
            var left = Option.Return(42);
            var right = Option<int>.None;

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Test]
        public void ConcatRightOnly()
        {
            var left = Option<int>.None;
            var right = Option.Return(42);

            var option = left.Concat(right);

            Assert.False(option.IsSome);
        }

        [Test]
        public void ConcatBoth()
        {
            var left = Option.Return(42);
            var right = Option.Return(42);

            var option = left.Concat(right);

            Assert.That(option.ForceValue(), Is.EqualTo(new Tuple<int,int>(42, 42)));
        }

        [Test]
        public void WhereWithMatchingPredicateReturnsSome()
        {
            var option = Option.Return(42);

            var result = option.Where(i => true);

            Assert.That(result, Is.EqualTo(option));
        }

        [Test]
        public void WhereWithFailingPredicateReturnsNone()
        {
            var option = Option.Return(42);

            var result = option.Where(i => false);

            Assert.That(result, Is.Not.EqualTo(option));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        }

        [Test]
        public void EmptyCallbackForSome()
        {
            var option = Option.Return(42);

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Test]
        public void EmptyCallbackForNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.Empty(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Test]
        public void CallbackForWhenSomeOnSome()
        {
            var option = Option.Return(42);

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.That(i, Is.EqualTo(option.ForceValue()));
        }

        [Test]
        public void CallbackForWhenSomeOnNone()
        {
            var option = Option<int>.None;

            var i = 666;

            option.WhenSome(j => i = j);

            Assert.That(i, Is.EqualTo(666));
        }

        [Test]
        public void CallbackForWhenNoneOnSome()
        {
            var option = Option.Return(42);

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.False(wasCalled);
        }

        [Test]
        public void CallbackForWhenNoneOnNone()
        {
            var option = Option<int>.None;

            var wasCalled = false;

            option.WhenNone(() => wasCalled = true);

            Assert.True(wasCalled);
        }

        [Test]
        public void TrueOperatorOnSome()
        {
            if (Option.Return(42)) { }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void TrueOperatorOnNone()
        {
            if (Option<int>.None)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void SelectManyBothSome()
        {
            var result = 
                from a in Option.Return(21)
                from b in Option.Return(21)
                select a + b;

            Assert.That(result.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void SelectManyFirstNone()
        {
            var result = 
                from a in Option<int>.None
                from b in Option.Return(21)
                select a + b;

            Assert.False(result.IsSome);
        }

        [Test]
        public void SelectManySecondNone()
        {
            var result =
                from a in Option.Return(21)
                from b in Option<int>.None
                select a + b;

            Assert.False(result.IsSome);
        }

        [Test]
        public void FirstOrDefaultEmptyList()
        {
            var items = new List<Option<int>>();
            var result = items.FirstOrDefault();

            Assert.False(result.IsSome);
        }

        [Test]
        public void FirstOrDefaultFirstSome()
        {
            var items = new List<Option<int>>{ Option.Return(42), Option.Return(666)};
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.That(result.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void FirstOrDefaultSecondSome()
        {
            var items = new List<Option<int>> { Option<int>.None, Option.Return(666) };
            var result = items.FirstOrDefault();

            Assert.True(result.IsSome);
            Assert.That(result.ForceValue(), Is.EqualTo(666));
        }

        [Test]
        public void FirstOrDefaultAllNone()
        {
            var items = new List<Option<int>> { Option<int>.None, Option<int>.None };
            var result = items.FirstOrDefault();

            Assert.False(result.IsSome);
        }

        [Test]
        public void ThenWithTrueValue()
        {
            var option = true.Then(() => 42);

            Assert.True(option.IsSome);
            Assert.That(option.ForceValue(), Is.EqualTo(42));
        }

        [Test]
        public void ThenWithFalseValue()
        {
            var option = false.Then(() => 42);

            Assert.False(option.IsSome);
        }

        [Test]
        public void NoneIfNullNonNull()
        {
            var str = "hello, world";

            var option = str.NoneIfNull();

            Assert.True(option.IsSome);
            Assert.That(option.ForceValue(), Is.EqualTo(str));
        }

        [Test]
        public void NoneIfNullNull()
        {
            string str = null;

            var option = str.NoneIfNull();

            Assert.False(option.IsSome);
        }

        [Test]
        public void NoneIfFalseOnFalse()
        {
            var option = false.NoneIfFalse();

            Assert.False(option.IsSome);
        }

        [Test]
        public void NoneIfFalseOnTrue()
        {
            var option = true.NoneIfFalse();

            Assert.True(option.IsSome);
        }

        [Test]
        public void TryGetValueForPresentKey()
        {
            var key = "hello";
            var expectedValue = 42;

            var dict = new Dictionary<string, object> {{key, expectedValue}};

            var result = dict.MaybeGetValue(key);
            
            Assert.That(result.IsSome);
            Assert.That(result.ForceValue(), Is.EqualTo(expectedValue));
        }

        [Test]
        public void TryGetValueForMissingKey()
        {
            var key = "hello";
            var expectedValue = 42;

            var dict = new Dictionary<string, object> {{"irrelevent", expectedValue}};

            var result = dict.MaybeGetValue(key);
            
            Assert.That(!result.IsSome);
        }
        [Test]
        public void TryGetValuesForPresentKey()
        {
            var key = "hello";
            var expectedValues = new[] {11, 42};

            var lookup = new[] {Tuple.Create("hello", 11), Tuple.Create("hello", 42)}
                .ToLookup(t => t.Item1, t => t.Item2);

            var result = lookup.MaybeGetValues(key);

            Assert.That(result.IsSome);
            Assert.That(result.ForceValue(), Is.EqualTo(expectedValues));
        }

        [Test]
        public void TryGetValuesForMissingKey()
        {
            var key = "hello";

            var lookup = new[] { Tuple.Create("irrelevant", 11), Tuple.Create("irrelevant", 42) }
               .ToLookup(t => t.Item1, t => t.Item2);

            var result = lookup.MaybeGetValues(key);

            Assert.That(result.IsSome, Is.False);
        }

        [Test]
        public void AggregateWithExistingMembers()
        {
            var list = new[] {1, 2, 3};

            var result = list.AggregateOrNone((total, current) => total + current);

            Assert.That(result.ForceValue(), Is.EqualTo(6));
        }

        [Test]
        public void AggregateWithMissingMembers()
        {
            var list = Enumerable.Empty<int>();

            var result = list.AggregateOrNone((total, current) => total + current);

            Assert.That(!result.IsSome);
        }

        [Test]
        public void AggregateWithOptions()
        {
            var list = new[] {1.AsOption(), 2.AsOption(), 3.AsOption()};

            var result = list.AggregateOrNone((total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.That(result.ForceValue(), Is.EqualTo(6));
        }

        [Test]
        public void AggregateWithOptionsAndNone()
        {
            var list = new[] { 1.AsOption(), Option<int>.None, 3.AsOption() };

            var result = list.AggregateOrNone((total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.That(!result.IsSome); ;
        }

        [Test]
        public void AggregateWithExistingMembersAndSeed()
        {
            var list = new[] { 1, 2, 3 };

            var result = list.AggregateOrNone(10, (total, current) => total + current);

            Assert.That(result.ForceValue(), Is.EqualTo(16));
        }

        [Test]
        public void AggregateWithMissingMembersAndSeed()
        {
            var list = Enumerable.Empty<int>();

            var result = list.AggregateOrNone(10, (total, current) => total + current);

            Assert.That(!result.IsSome);
        }
        [Test]
        public void AggregateWithOptionsAndSeed()
        {
            var list = new[] { 1.AsOption(), 2.AsOption(), 3.AsOption() };

            var result = list.AggregateOrNone(10.AsOption(), (total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.That(result.ForceValue(), Is.EqualTo(16));
        }

        [Test]
        public void AggregateWithOptionsAndNoneAndSeed()
        {
            var list = new[] { 1.AsOption(), Option<int>.None, 3.AsOption() };

            var result = list.AggregateOrNone(10.AsOption(), (total, current) => total.SelectMany(t => current.Map(c => t + c)));

            Assert.That(!result.IsSome);
        }
        [Test]
        public void ImplicitCastBool()
        {
            bool b  = "IsSome".AsOption();
            Assert.True(b);
            b = Option<string>.None;
            Assert.False(b);
        }

        [Test]
        public void ImplicitCastNull()
        {
            string n = null;
            Option<string> none = n;

            Assert.False(none);
        }

        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(1, null, 1)]
        [TestCase(null, 2, 2)]
        [TestCase(null, null, null)]
        public void PipeOperator(int? x, int? y, int? expected)
        {
            Assert.That(expected.NoneIfEmpty(), Is.EqualTo(x.NoneIfEmpty() | y.NoneIfEmpty()));
        }

        [Test]
        public void PipeOperatorShortCircuit()
        {
            var some = new object().AsOption();
            var none = Option<object>.None;
            Func<Option<object>> fail = () => { throw new Exception(); };

            Assert.DoesNotThrow(() => { var result = some | fail; });
            Assert.Throws<Exception>(() => { var result = none | fail; });
        }

#if NET45
        [Test]
        public async Task DoAsyncWithSome()
        {
            var some = new object().AsOption();
            var wasCalled = false;

            await some.DoAsync(async x => {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.True(wasCalled);
        }

        [Test]
        public async Task DoAsyncWithNone()
        {
            var none = Option<object>.None;
            var wasCalled = false;

            await none.DoAsync(async x => {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.False(wasCalled);
        }

        [Test]
        public async Task DoAsyncWithNull()
        {
            Option<object> none = null;
            var wasCalled = false;

            await none.DoAsync(async x => {
                await Task.Delay(0);
                wasCalled = true;
            });

            Assert.False(wasCalled);
        }

        [Test]
        public async Task MapAsyncWithSome()
        {
            var some = 0.AsOption();

            var result = await some.MapAsync(
                async x => await Task.FromResult(x + 1));

            Assert.AreEqual(1.AsOption(), result);
        }

        [Test]
        public async Task MapAsyncWithNone()
        {
            var none = Option<int>.None;

            var result = await none.MapAsync(
                async x => await Task.FromResult(x + 1));

            Assert.AreEqual(Option<int>.None, result);
        }

        [Test]
        public async Task MapAsyncWithNull()
        {
            Option<object> none = null;

            var result = await none.MapAsync(
                async x => await Task.FromResult(0));

            Assert.AreEqual(Option<int>.None, result);
        }
#endif

#region Monad laws
        [Test]
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

            Assert.That(viaOption, Is.EqualTo(straight));
        }

        [Test]
        public void RightIdentity()
        {
            // forall m. m >>= return === m
            // if we call selectmany with the return function
            // the result should be equal to the option we started with

            var someName = "Wadler";
            var option = Option.Return(someName);

            var viaOption = option.SelectMany(Option.Return);

            Assert.That(viaOption, Is.EqualTo(option));
        }

        [Test]
        public void Associativity()
        {
            // Order of application of functions must associate

            var someName = "Wadler";
            var option = Option.Return(someName);

            Func<string,Option<string>> f;
            Func<string,Option<string>> g;

            f = s => s.ToUpper();
            g = s => s.ToLower();

            var first = option.SelectMany(f).SelectMany(g);
            var second = option.SelectMany(x => f(x).SelectMany(g));

            Assert.That(first, Is.EqualTo(second));
        }
#endregion
    }
}
