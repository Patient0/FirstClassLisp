using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    sealed class StackFunctionAdapter : AbstractStackFunction
    {
        private readonly Function function;
        public StackFunctionAdapter(Function function)
        {
            this.function = function;
        }

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            return c.PushResult(function.Evaluate(args));
        }

        public override string ToString()
        {
            return function.ToString();
        }
    }

    static class FunctionExtensions
    {
        public static StackFunction ToStack(this Function f)
        {
            return new StackFunctionAdapter(f);
        }
    }
}
