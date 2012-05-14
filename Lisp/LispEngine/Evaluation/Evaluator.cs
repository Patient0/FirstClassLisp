using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class Evaluator
    {
        public object evaluate(Environment env, Datum datum)
        {
            // Atoms evaluate to themselves
            var a = datum as Atom;
            if (a != null)
                return a.Value;
            var s = datum as Symbol;
            if(s != null)
                return env.lookup(s.Identifier);
            
            return null;
        }
    }
}
