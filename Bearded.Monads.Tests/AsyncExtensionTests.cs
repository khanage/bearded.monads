using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bearded.Monads.Tests
{
    public class AsyncExtensionTests
    {
        [Test]
        public async void SelectMany_Happy()
        {
            var expected = 20;

            var actual = await (
                from x in Task.FromResult(10)
                from y in Task.FromResult(10)
                select x + y
            );

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
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
            
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(failureWindow));
        }
        

        [Test]
        public async void SelectMany_Exceptions()
        {
            var runner = 
                from x in Task.Factory.StartNew<int>(() => { Thread.Sleep(100); throw new Exception("X"); })
                from y in Task.Factory.StartNew<int>(() => { throw new Exception("Y"); })
                select x + y;

            try
            {
                await runner;
                Assert.Fail("Expecting exception");
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.EqualTo("X"));
            }
        }
    }
}