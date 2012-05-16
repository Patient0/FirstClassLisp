using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class Evaluator : DatumHelpers
    {
        public object evaluate(Environment env, Datum datum)
        {
            var a = datum as Atom;
            if (a != null)
                return a.Value;
            var s = datum as Symbol;
            if(s != null)
                return env.lookup(s.Identifier);
            var c = datum as Pair;
            if(c != null)
            {
                var first = evaluate(env, c.First);
                var m = first as Macro;
                if(m != null)
                    return evaluate(env, m.Expand(this, env, c.Second));
                var f = first as Function;
                if (f == null)
                    throw new Exception(string.Format("'{0}' is not a function", first));
                var argDatums = enumerate(c.Second);
                var args = argDatums.Select(x => evaluate(env, x));
                return f.evaluate(args);
            }
            
            return null;
        }
    }
}
