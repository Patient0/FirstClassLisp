using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Cons : BinaryFunction
    {
        public static readonly StackFunction Instance = new Cons().ToStack();

        protected override Datum eval(Datum arg1, Datum arg2)
        {
            return DatumHelpers.cons(arg1, arg2);
        }
    }
}
