using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class FunctionExpression : DatumHelpers, FExpression
    {
        private readonly Function function;

        private static Datum evaluateArgs(Evaluator evaluator, Environment env, Datum datum)
        {
            return compound(enumerate(datum).Select(f => evaluator.Evaluate(env, f)).ToArray());
        }

        public FunctionExpression(Function function)
        {
            this.function = function;
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            return function.Evaluate(evaluator, evaluateArgs(evaluator, env, args));
        }

        // Until we've resolved all functions into stack functions
        // themselves, use our own copy of "Evaluator"
        private static readonly Evaluator e = new Evaluator();
        private class InvokeFunction : Task
        {
            private readonly Function function;
            private readonly int argCount;
            public InvokeFunction(Function function, int argCount)
            {
                this.function = function;
                this.argCount = argCount;
            }

            public void Perform(EvaluatorStack stack)
            {
                var args = new Datum[argCount];
                for(var i = 0; i < argCount; ++i)
                {
                    args[i] = stack.PopResult();
                }
                // TODO: Functions themselves also have access to the EvaluatorStack
                var result = function.Evaluate(e, compound(args));
                stack.PushResult(result);
            }
        }

        public void Evaluate(EvaluatorStack stack, Environment env, Datum args)
        {
            var argArray = enumerate(args).ToArray();
            stack.PushTask(new InvokeFunction(function, argArray.Length));
            foreach (var arg in argArray)
            {
                stack.PushTask(new EvaluateTask(arg, env));
            }
        }
    }
}
