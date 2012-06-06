using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Stack;

namespace LispEngine.Evaluation
{
    sealed class StackFunctionAdapter : StackFunction
    {
        private readonly Function function;
        public StackFunctionAdapter(Function function)
        {
            this.function = function;
        }

        public void Evaluate(EvaluatorStack s, Datum args)
        {
            s.PushResult(function.Evaluate(args));
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
