using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    public class DatumHelpers
    {
        public static readonly Datum nil = Null.Instance;

        public static readonly Datum quote = symbol("quote");
        public static readonly Datum quasiquote = symbol("quasiquote");
        public static readonly Datum unquote = symbol("unquote");
        public static readonly Datum unquoteSplicing = symbol("unquote-splicing");
        public static readonly Datum dot = symbol("dot");
        public static readonly Datum slash = symbol("slash");

        public const string quoteAbbreviation = "'";
        public const string quasiquoteAbbreviation = "`";
        public const string splicingAbbreviation = ",@";
        public const string unquoteAbbreviation = ",";

        public static Datum isQuote(string abbreviation)
        {
            if (abbreviation == unquoteAbbreviation)
                return unquote;
            if (abbreviation == quasiquoteAbbreviation)
                return quasiquote;
            if (abbreviation == splicingAbbreviation)
                return unquoteSplicing;
            if (abbreviation == quoteAbbreviation)
                return quote;
            return null;
        }

        public static string isQuote(Datum d)
        {
            if (d.Equals(quote))
                return quoteAbbreviation;
            if (d.Equals(quasiquote))
                return quasiquoteAbbreviation;
            if (d.Equals(unquoteSplicing))
                return splicingAbbreviation;
            if (d.Equals(unquote))
                return unquoteAbbreviation;
            return null;
        }

        public static Exception error(string msg, params object[] args)
        {
            return new Exception(string.Format(msg, args));
        }

        public static Symbol castSymbol(Datum dt)
        {
            var symbol = dt as Symbol;
            if (symbol == null)
                throw new Exception(String.Format("'{0}' is not a symbol", dt));
            return symbol;
        }

        public static Symbol symbol(string identifier)
        {
            return Symbol.GetSymbol(identifier);
        }

        public static Pair cons(Datum first, Datum second)
        {
            return new Pair(first, second);
        }

        public static Pair setCar(Datum first, Datum val)
        {
            var p = castPair(first);
            p.First = val;
            return p;
        }

        public static Pair setCdr(Datum first, Datum val)
        {
            var p = castPair(first);
            p.Second = val;
            return p;
        }

        public static Datum atomList<T>(params T[] e) 
        {
            return compound(e.AsEnumerable().Select(atom).ToArray<Datum>());
        }

        public static Datum compound(params Datum[] e)
        {
            var list = new List<Datum>(e);
            list.Reverse();
            return list.Aggregate(nil, (current, l) => cons(l, current));
        }

        public static Vector vector(params Datum[] elements)
        {
            return new Vector(elements);
        }

        public static Atom atom<T>(T value)
        {
            return new Atom(value);
        }

        private static Pair castPair(Datum d)
        {
            var pair = d as Pair;
            if (pair == null)
                throw error("'{0}' is not a pair", d);
            return pair;
        }

        public static Vector castVector(Datum d)
        {
            var v = d as Vector;
            if (v == null)
                throw error("Expected '{0}' to be a vector", d);
            return v;
        }

        public static Datum car(Datum d)
        {
            return castPair(d).First;
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

        public static object castObject(Datum d)
        {
            var a = d as Atom;
            if (a == null)
                throw error("Expected '{0}' to be an atom, but got '{1}' instead", d, d.GetType().Name);
            return a.Value;
        }

        public static int castInt(Datum d)
        {
            var value = castObject(d);
            return (int) value;
        }

        public static string castString(Datum datum)
        {
            return (string)castObject(datum);
        }

        public static IEnumerable<object> atoms(Datum list)
        {
            return enumerate(list).Select(castObject);
        }

        public static IEnumerable<int> enumerateInts(Datum list)
        {
            return atoms(list).Select(v => (int) v);
        }

    }
}
