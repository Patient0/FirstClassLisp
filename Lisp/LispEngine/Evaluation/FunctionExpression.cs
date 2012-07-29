using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

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

            public Continuation Perform(Continuation c)
            {
                var argResults = DatumHelpers.nil;
                for (var i = 0; i < argCount; ++i)
                {
                    argResults = DatumHelpers.cons(c.Result, argResults);
                    c = c.PopResult();
                }
                return function.Evaluate(c, argResults);
            }

            public override string ToString()
            {
                return string.Format("Invoke '{0}' with {1} args", function, argCount);
            }
        }

        public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
        {
            var argArray = DatumHelpers.enumerate(args).ToArray();
            Array.Reverse(argArray);
            c = c.PushTask(new InvokeFunction(function, argArray.Length));
            return argArray.Aggregate(c, (current, arg) => current.Evaluate(env, arg));
        }
    }
}
