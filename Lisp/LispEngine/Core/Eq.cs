using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Eq : DatumHelpers, Function
    {
        public static readonly Function Instance = new Eq();
        public Datum Evaluate(Evaluator evaluator, Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length != 2)
                throw error("eq?: Expected 2 arguments, got {0}", a.Length);
            var left = a[0] as Atom;
            var right = a[1] as Atom;
            if(left == null || right == null)
                return atom(false);
            return atom(left.Equals(right));
        }
    }
}
