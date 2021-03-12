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

        public static async Task<Try<C>> SelectMany<A, B, C>(this Task<Try<A>> task, Func<A, Task<Try<B>>> bind, Func<A, B, C> project)
        {
            try
            {
                var ta = await task.Run();
                if (ta.IsError) return ta.AsError().Value;

                var a = ta.AsSuccess().Value;

                var tb = await bind(a).Run();

                if (tb.IsError) return tb.AsError().Value;

                var b = tb.AsSuccess().Value;

                return project(a, b);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static async Task<Try<B>> SelectMany<A, B>(this Task<Try<A>> task, Func<A, Task<Try<B>>> bind)
        {
            try
            {
                return await task.SelectMany(bind, (_, b) => b);
            }
            catch (Exception e)
            {
                return e;
            }
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

        public static Task<Try<B>> Select<A, B>(this Task<Try<A>> task, Func<A, B> callback)
        {
            return task.ContinueWith(t => t.Result.Map(callback), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public static Task<IEnumerable<B>> Traverse<A, B>(this IEnumerable<A> elems, Func<A, Task<B>> callback)
            => Task.WhenAll(elems.Select(e => callback(e).Run()))
                .Select(x => (IEnumerable<B>)x);

        public static Task<Try<IEnumerable<B>>> Traverse<A, B>(this IEnumerable<A> elems, Func<A, Task<Try<B>>> callback)
            => Task.WhenAll(elems.Select(e => callback(e).Run()))
                .Select(enumTry =>
                {
                    var maybeError = enumTry.FirstOrNone(x => x.IsError).Select(x => x.AsError().Value);

                    return !maybeError.IsSome
                        ? Try<IEnumerable<B>>.Create(enumTry.Select(x => x.AsSuccess().Value))
                        : maybeError.ForceValue();
                });


        public static Task<IEnumerable<A>> Sequence<A>(this IEnumerable<Task<A>> tasks)
            => tasks.Traverse(id);

        public static Task<Try<A>> AsTask<A>(this A a) => Task.FromResult(a.AsTry());

        public static Task<Try<A>> AsTask<A>(this Try<A> a) => Task.FromResult(a);
    }
}
