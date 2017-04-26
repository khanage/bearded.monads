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

        public static Option<bool> MaybeBool(this string value)
        {
            bool val;

            if (bool.TryParse(value, out val))
                return val;

            return Option<bool>.None;
        }

        public static Option<float> MaybeFloat(this string value)
        {
            float val;

            if (float.TryParse(value, out val))
                return val;

            return Option<float>.None;
        }

        public static Option<Uri> MaybeUri(this string value, UriKind? uriKind)
        {
            var kindToUse = uriKind ?? UriKind.RelativeOrAbsolute;

            Uri uri;

            if (Uri.TryCreate(value, kindToUse, out uri))
                return uri;

            return Option<Uri>.None;
        }

        public static Option<A> MaybeEnum<A>(this string value, bool ignoreCase = false)
            where A : struct
        {
            A val;

            if (Enum.TryParse(value, ignoreCase, out val))
                return val;

            return Option<A>.None;
        }

        public static Option<A> MaybeEnum<A>(this string value)
            where A : struct
        {
            return value.MaybeEnum<A>(false);
        }
    }
}
