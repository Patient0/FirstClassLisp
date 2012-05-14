using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public class DatumHelpers
    {
        public static readonly Datum nil = Null.Instance;
        public static Symbol symbol(string identifier)
        {
            return new Symbol(identifier);
        }

        public static Pair cons(Datum first, Datum second)
        {
            return new Pair(first, second);
        }

        public static Datum compound(params Datum[] e)
        {
            var list = new List<Datum>(e);
            list.Reverse();
            return list.Aggregate(nil, (current, l) => cons(l, current));
        }

        public static Atom atom(Object value)
        {
            return new Atom(value);
        }
    }
}
