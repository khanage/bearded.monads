using System;

namespace Bearded.Monads
{
    public static class MaybeParseExtensions
    {
        public static Option<int> MaybeInt(this string value)
        {
            int val;

            if (int.TryParse(value, out val))
                return val;

            return Option<int>.None;
        }

        public static Option<float> MaybeFloat(this string value)
        {
            int val;

            if (int.TryParse(value, out val))
                return val;

            return Option<float>.None;
        }

#if NET4
        public static Option<A> MaybeEnum<A>(this string value, bool ignoreCase = false)
            where A : struct
        {
            A val;

            if (Enum.TryParse(value, ignoreCase, out val))
                return val;

            return Option<A>.None;
        }
#else
        public static Option<A> MaybeEnum<A>(this string value)
            where A : struct
        {
            return value.MaybeEnum<A>(false);
        }

        public static Option<A> MaybeEnum<A>(this string value, bool ignoreCase)
            where A : struct
        {
            try
            {
                return (A)Enum.Parse(typeof(A), value, ignoreCase);
            }
            catch (Exception)
            {
                return Option<A>.None;
            }
        }
#endif
    }
}
