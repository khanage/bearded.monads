using System;
using System.Threading.Tasks;

namespace Bearded.Monads
{

    public static class AsyncExtensions
    {
        public static async Task<C> SelectMany<A, B, C>(this Task<A> task, Func<A, Task<B>> bind, Func<A, B, C> project)
        {
            var a = await task.Run();
            var b = await bind(a).Run();

            return project(a, b);
        }

        public static Task<A> Run<A>(this Task<A> task)
        {
            if (task.Status == TaskStatus.Created) task.Start();
            return task;
        }

        public static Task<B> Select<A, B>(this Task<A> task, Func<A, B> callback)
        {
            return task.ContinueWith(t => callback(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
