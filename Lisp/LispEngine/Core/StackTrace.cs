using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class StackTrace : UnaryFunction
    {
        protected override Datum eval(Datum arg)
        {
            var cfunction = arg as CallCC.ContinuationFunction;
            if (cfunction == null)
                throw DatumHelpers.error("'{0}' is not a continuation", arg);
            var stack = DatumHelpers.nil;
            var c = cfunction.Continuation;
            while(c.Task != null)
            {
                stack = DatumHelpers.cons(c.Task.ToString().ToAtom(), stack);
                c = c.PopTask();
            }
            return stack;
        }

        public override string ToString()
        {
            return ",stack-trace";
        }

        public static readonly StackFunction Instance = new StackTrace().ToStack();
    }
}
