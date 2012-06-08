using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class FunctionExpression : AbstractFExpression
    {
        private readonly StackFunction function;

        public FunctionExpression(StackFunction function)
        {
            this.function = function;
        }

        private class InvokeFunction : Task
        {
            private readonly StackFunction function;
            private readonly int argCount;
            public InvokeFunction(StackFunction function, int argCount)
            {
                this.function = function;
                this.argCount = argCount;
            }

            public void Perform(EvaluatorStack stack)
            {
                var args = new Datum[argCount];
                for(var i = 0; i < argCount; ++i)
                   args[i] = stack.PopResult();
                function.Evaluate(stack, DatumHelpers.compound(args));
            }
        }

        public override void Evaluate(EvaluatorStack stack, Environment env, Datum args)
        {
            var argArray = DatumHelpers.enumerate(args).ToArray();
            stack.PushTask(new InvokeFunction(function, argArray.Length));
            foreach (var arg in argArray)
                stack.Evaluate(env, arg);
        }
    }
}
