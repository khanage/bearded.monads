using System.Collections.Generic;
using System.Linq;
using static Bearded.Monads.Syntax;

namespace Bearded.Monads
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<A> Flatten<A>(
            this IEnumerable<IEnumerable<A>> incoming)
            => incoming.SelectMany(id);
    }
}
