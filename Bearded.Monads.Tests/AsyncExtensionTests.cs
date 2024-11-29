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
        public async void SelectManyTry_Happy()
        {
            async Task<Try<int>> TryResult(int value)
            {
                return await Task.FromResult(value.AsTry());
            }
            
            var expected = 20;

            var actual = await (
                from x in TryResult(10)
                from y in TryResult(10)
                select x + y
            );

            Assert.Equal(expected, actual.AsSuccess().Value);
        }

        [Fact]
        public async void SelectManyTry_Error()
        {
            var executionCount = 0;

            async Task<Try<int>> TryResult(int value)
            {
                executionCount++;
                if (value > 10) 
                    return new Exception("Failed");
                
                return await Task.FromResult(value.AsTry());
            }
            
            var actual = await (
                from x in TryResult(13)
                from y in TryResult(10)
                select x + y
            );

            Assert.True(actual.IsError);
            Assert.Equal(1, executionCount);
        }


        [Fact]
        public async void Traverse_Happy()
        {
            var incoming = Enumerable.Range(1, 2);
            var result = await incoming
                .Traverse(i => Task.FromResult(i * 20))
                .Select(x => x.ToList());

            Assert.Equal(2, result.Count);
            Assert.Equal(20, result[0]);
            Assert.Equal(40, result[1]);
        }
        
        [Fact]
        public async void TraverseTry_Happy()
        {
            async Task<Try<int>> TryResult(int value)
            {
                return await Task.FromResult(value.AsTry());
            }

            var incoming = Enumerable.Range(1, 2);
            var result = await incoming
                .Traverse(i => TryResult(i * 20))
                .Select(x => x.ToList());

            var resultForce = result.AsSuccess().Value;
            Assert.Equal(2, resultForce.Count);
            Assert.Equal(20, resultForce[0]);
            Assert.Equal(40, resultForce[1]);
        }

        [Fact]
        public async void TraverseTry_Unhappy()
        {
            async Task<Try<int>> TryResult(int value)
            {
                if (value > 20)
                    return new Exception("Failed");
                return await Task.FromResult(value.AsTry());
            }

            var incoming = Enumerable.Range(1, 2);
            var result = await incoming
                .Traverse(i => TryResult(i * 20))
                .Select(x => x.ToList());

            Assert.True(result.IsError);
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
