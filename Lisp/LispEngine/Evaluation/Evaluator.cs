using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class Evaluator : DatumHelpers
    {
        private Datum evaluateArgs(Environment env, Datum datum)
        {
            return compound(enumerate(datum).Select(f => evaluate(env, f)).ToArray());
        }

        public Datum evaluate(Environment env, Datum datum)
        {
            var a = datum as Atom;
            if (a != null)
                return a;
            var s = datum as Symbol;
            if(s != null)
                return env.lookup(s.Identifier);
            var c = datum as Pair;
            if(c != null)
            {
                var first = evaluate(env, c.First);
                var m = first as Macro;
                if(m != null)
                    return m.Expand(this, env, c.Second);
                var f = first as Function;
                if (f == null)
                    throw new Exception(string.Format("'{0}' is not a function", first));
                return f.Evaluate(this, evaluateArgs(env, c.Second));
            }
            
            return null;
        }
    }
}
