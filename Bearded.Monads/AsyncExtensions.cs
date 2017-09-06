using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Bearded.Monads.Syntax;

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

        public static Task<B> SelectMany<A, B>(this Task<A> task, Func<A, Task<B>> bind)
            => task.SelectMany(bind, (_, b) => b);

        public static Task<A> Run<A>(this Task<A> task)
        {
            if (task.Status == TaskStatus.Created) task.Start();
            return task;
        }

        public static Task<B> Select<A, B>(this Task<A> task, Func<A, B> callback)
        {
            return task.ContinueWith(t => callback(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static Task<IEnumerable<B>> Traverse<A, B>(this IEnumerable<A> elems, Func<A, Task<B>> callback)
            => Task.WhenAll(elems.Select(e => callback(e).Run()))
                .Select(x => (IEnumerable<B>)x);

        public static Task<IEnumerable<A>> Sequence<A>(this IEnumerable<Task<A>> tasks)
            => tasks.Traverse(id);
    }
}
