using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    public static class DatumExtensions
    {
        public static IEnumerable<Datum> Enumerate(this Datum list)
        {
            return DatumHelpers.enumerate(list);
        }

        public static Datum[] ToArray(this Datum list)
        {
            return list.Enumerate().ToArray();
        }

        public static Datum ToList(this IEnumerable<Datum> datums)
        {
            return DatumHelpers.compound(datums.ToArray());
        }

        public static Datum ToAtom(this object o)
        {
            return DatumHelpers.atom(o);
        }

        public static Symbol ToSymbol(this string identifier)
        {
            return DatumHelpers.symbol(identifier);
        }

        public static Symbol CastSymbol(this Datum d)
        {
            return DatumHelpers.castSymbol(d);
        }

        public static string CastString(this Datum d)
        {
            return DatumHelpers.castString(d);
        }

        public static int CastInt(this Datum d)
        {
            return DatumHelpers.castInt(d);
        }

        public static object CastObject(this Datum d)
        {
            return DatumHelpers.castObject(d);
        }
    }
}
