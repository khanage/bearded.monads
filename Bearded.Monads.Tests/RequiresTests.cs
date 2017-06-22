using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class RequiresTests
    {
        [Fact]
        public void In()
        {
            int method(int i) => i + 1;
            var requires = Requires<int>.In(method);
            var actual = requires.Run(1);
            var expected = method(1); // 2

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Join()
        {
            int firstMethod(int i) => i + 1;
            int secondMethod(int i, int j) => i * j;
            Requires<int, int> Second(int i) => Requires<int>.In(j => secondMethod(i, j));

            var first = Requires<int>.In(firstMethod);

            var actual = first.Join(Second).Run(2);
            var expected = secondMethod(2, firstMethod(2)); // 6

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Select()
        {
            int method(int i) => i + 1;

            var requires = Requires<int>.In(method);

            var actual = requires.Select(i => i * 2).Run(1);
            var expected = method(1) * 2; // 4

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DoOneParam()
        {
            int method(int i) => i + 1;

            var sideEffect = new List<int>();

            var requires = Requires<int>.In(method);
            requires.Do(sideEffect.Add).Run(1);

            Assert.Contains(2, sideEffect);
            Assert.Equal(1, sideEffect.Count);
        }

        [Fact]
        public void DoTwoParam()
        {
            int method(int i) => i + 1;

            var sideEffect = new List<int>();

            var requires = Requires<int>.In(method);
            requires.Do((i, j) =>
            {
                sideEffect.Add(i);
                sideEffect.Add(j);
            }).Run(1);

            Assert.Contains(1, sideEffect);
            Assert.Contains(2, sideEffect);
            Assert.Equal(2, sideEffect.Count);
        }

        [Fact]
        public void SelectMany()
        {
            int method1(int i) => i + 1;
            int method2(int i) => i * 2;

            var requiresAddition = Requires<int>.In(method1);
            var requiresMultiplication = Requires<int>.In(method2);

            var actual = (
                from x in requiresAddition
                from y in requiresMultiplication
                select x + y
            ).Run(2);

            var expected = method1(2) + method2(2);

            Assert.Equal(expected, actual);
        }
    }
}
