using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    /**
     * Turns a function into something that
     * can be used as a macro.
     */
    internal class Macro : DatumHelpers, Function
    {
        public static readonly StackFunction Instance = new Macro().ToStack();

        private sealed class EvaluateExpansion : Task
        {
            private readonly Environment env;

            public EvaluateExpansion(Environment env)
            {
                this.env = env;
            }

            public void Perform(EvaluatorStack stack)
            {
                var expansion = stack.PopResult();
                stack.Evaluate(env, expansion);
            }
        }

        private class MacroClosure : AbstractFExpression
        {
            private readonly StackFunction argFunction;

            public MacroClosure(StackFunction argFunction)
            {
                this.argFunction = argFunction;
            }

            public override void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
            {
                evaluator.PushTask(new EvaluateExpansion(env));
                argFunction.Evaluate(evaluator, args);
            }
        }

        public static FExpression ToMacro(StackFunction function)
        {
            return new MacroClosure(function);
        }

        public Datum Evaluate(Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length != 1)
                // TODO: Move this error boiler plate into DatumHelpers
                throw error("Incorrect arguments. Expected {0}, got {1}", 1, a.Length);
            var function = a[0] as StackFunction;
            if (function == null)
                throw error("Expected function argument");
            return ToMacro(function);
        }
    }
}
