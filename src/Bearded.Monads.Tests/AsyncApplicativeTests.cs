using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using static Bearded.Monads.AsyncApplicative;

namespace Bearded.Monads.Tests
{
    class AsyncApplicativeTests
    {
        [Test]
        public async void BaseAsynquenceCall()
        {
            var result = await Asynquence(Task.FromResult(10), Task.FromResult(20))
                .Select((x, y) => x + y);
            var expected = 30;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void ChainedCall_2()
        {
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(20))
                .Select((x, y) => x + y);
            var expected = 30;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void ChainedCall_3()
        {
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .Select((x, y, z) => x + y + z);
            var expected = 30;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void ChainedCall_4()
        {
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .Select((a, b, c, d) => a + b + c + d);
            var expected = 40;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void ChainedCall_5()
        {
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .Select((a, b, c, d, e) => a + b + c + d + e);
            var expected = 50;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void ChainedCall_6()
        {
            var result = await Asynquence(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .And(Task.FromResult(10))
                .Select((a, b, c, d, e, f) => a + b + c + d + e + f);
            var expected = 60;

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async void Calls_Concurrent()
        {
            var millisecondsPerCall = 200;
            var sw = Stopwatch.StartNew();

            await Asynquence(Task.Delay(millisecondsPerCall).ContinueWith(_ => 1))
                .And(Task.Delay(millisecondsPerCall).ContinueWith(_ => 1))
                .And(Task.Delay(millisecondsPerCall).ContinueWith(_ => 1))
                .Select((x, y, z) => 1);

            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, 
                Is.GreaterThanOrEqualTo(millisecondsPerCall)
                 .And.LessThanOrEqualTo(millisecondsPerCall * 2)
            );
        }

        [Test]
        public async void Calls_NotStartedTask()
        {
            var millisecondsPerCall = 200;
            var sw = Stopwatch.StartNew();
            var cancellation = new CancellationTokenSource();
            cancellation.CancelAfter(millisecondsPerCall * 3);

            await Asynquence(Task.Delay(millisecondsPerCall).ContinueWith(_ => 1))
                .And(new Task<int>(() => 1, cancellation.Token))
                .Select((x, y) => 1);

            sw.Stop();

            Assert.That(sw.ElapsedMilliseconds, 
                Is.LessThanOrEqualTo(millisecondsPerCall * 2)
            );
        }
    }
}
