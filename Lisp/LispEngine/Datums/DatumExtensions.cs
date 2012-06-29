using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
