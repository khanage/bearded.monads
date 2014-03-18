#if NET35
namespace Beard.Monads
{
    public class Tuple<A,B>
    {
        public A Item1 { get; set; }
        public B Item2 { get; set; }

        public Tuple(A a, B b)
        {
            Item1 = a;
            Item2 = b;
        }
    }
}
#endif