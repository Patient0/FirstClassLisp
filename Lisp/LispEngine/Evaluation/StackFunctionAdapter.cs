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

        public override Continuation Evaluate(Continuation s, Datum args)
        {
            return s.PushResult(function.Evaluate(args));
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
