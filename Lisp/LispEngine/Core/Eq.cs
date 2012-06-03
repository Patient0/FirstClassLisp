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
            if ((a[0] as Pair) != null ||
                (a[1] as Pair) != null)
                return atom(ReferenceEquals(a[0], a[1]));
            return atom(a[0].Equals(a[1]));
        }
    }
}
