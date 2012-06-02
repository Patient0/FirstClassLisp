using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class Evaluator : DatumHelpers
    {
        private static FExpression toFExpression(Datum d)
        {
            var fexpr = d as FExpression;
            if (fexpr != null)
                return fexpr;
            var function = d as Function;
            if (function != null)
                return new FunctionExpression(function);
            throw error("'{0}' is not callable", d);
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            var s = datum as Symbol;
            if(s != null)
                return env.Lookup(s.Identifier);
            var c = datum as Pair;
            if(c != null)
            {
                var first = Evaluate(env, c.First);
                var fexp = toFExpression(first);
                return fexp.Evaluate(this, env, c.Second);
            }
            // Anything else just evaluates to itself
            return datum;
        }
    }
}
