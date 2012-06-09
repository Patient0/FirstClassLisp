using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Eq : BinaryFunction
    {
        public static readonly StackFunction Instance = new Eq().ToStack();
        protected override Datum eval(Datum arg1, Datum arg2)
        {
            if ((arg1 as Pair) != null ||
                (arg2 as Pair) != null)
                return DatumHelpers.atom(ReferenceEquals(arg1, arg2));
            return DatumHelpers.atom(arg1.Equals(arg2));
        }
    }
}
