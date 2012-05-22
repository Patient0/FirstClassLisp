using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    public class DatumHelpers
    {
        public static readonly Datum nil = Null.Instance;

        public static readonly Datum quote = symbol("quote");

        public static Exception error(string msg, params object[] args)
        {
            return new Exception(string.Format(msg, args));
        }

        public static Exception error(Exception cause, string msg, params object[] args)
        {
            return new Exception(string.Format(msg, args), cause);
        }


        public static string getIdentifier(Datum dt)
        {
            var symbol = dt as Symbol;
            if (symbol == null)
                throw new Exception(String.Format("'{0}' is not a symbol", dt));
            return symbol.Identifier;
        }

        public static Symbol symbol(string identifier)
        {
            return new Symbol(identifier);
        }

        public static Pair cons(Datum first, Datum second)
        {
            return new Pair(first, second);
        }

        public static Datum atomList(params object[] e)
        {
            return compound(e.AsEnumerable().Select(atom).ToArray<Datum>());
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

        public static IEnumerable<Datum> enumerate(Datum list)
        {
            var next = list;
            while(next != nil)
            {
                var pair = next as Pair;
                if(pair == null)
                    throw new Exception("Not a list");
                next = pair.Second;
                yield return pair.First;
            }
        }
    }
}
