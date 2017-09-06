using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bearded.Monads.Tests
{
    public class AsyncExtensionTests
    {
        [Fact]
        public async void SelectMany_Happy()
        {
            var expected = 20;

            var actual = await (
                from x in Task.FromResult(10)
                from y in Task.FromResult(10)
                select x + y
            );

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void Traverse_Happy()
        {
            var expected = 20;

            var incoming = Enumerable.Range(1, 2);
            var result = await incoming
                .Traverse(i => Task.FromResult(i * 20))
                .Select(x => x.ToList());

            Assert.Equal(2, result.Count);
            Assert.Equal(20, result[0]);
            Assert.Equal(40, result[1]);
        }

        [Fact]
        public async void SelectMany_NotStarted()
        {
            var cancellation = new CancellationTokenSource();
            var failureWindow = 100;

            cancellation.CancelAfter(failureWindow);

            var sw = Stopwatch.StartNew();

            var expected = 20;

            var actual = await (
                from x in new Task<int>(() => 10, cancellation.Token)
                from y in new Task<int>(() => 10, cancellation.Token)
                select x + y
            );

            sw.Stop();

            Assert.Equal(expected, actual);
            Assert.True(sw.ElapsedMilliseconds < failureWindow);
        }


        [Fact]
        public async void SelectMany_Exceptions()
        {
            var runner =
                from x in Task.Factory.StartNew<int>(() => { Thread.Sleep(100); throw new Exception("X"); })
                from y in Task.Factory.StartNew<int>(() => { throw new Exception("Y"); })
                select x + y;

            try
            {
                await runner;
                throw new Exception("Expecting exception");
            }
            catch (Exception e)
            {
                Assert.Equal("X", e.Message);
            }
        }
    }
}
