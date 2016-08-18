using System;
using System.Collections.Generic;

#if NET40 
#else
namespace Bearded.Monads
{
    public class Tuple<A, B> : IEquatable<Tuple<A, B>>
    {
        public A Item1 { get; set; }
        public B Item2 { get; set; }

        public Tuple(A a, B b)
        {
            Item1 = a;
            Item2 = b;
        }

        public override string ToString()
        {
            return $"Item1: {Item1}, Item2: {Item2}";
        }

        public bool Equals(Tuple<A, B> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<A>.Default.Equals(Item1, other.Item1) && EqualityComparer<B>.Default.Equals(Item2, other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tuple<A, B>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<A>.Default.GetHashCode(Item1) * 397) ^ EqualityComparer<B>.Default.GetHashCode(Item2);
            }
        }

        public static bool operator ==(Tuple<A, B> left, Tuple<A, B> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tuple<A, B> left, Tuple<A, B> right)
        {
            return !Equals(left, right);
        }


    }
}
#endif