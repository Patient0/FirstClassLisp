using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
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

            public Continuation Perform(Continuation c)
            {
                var args = DatumHelpers.nil;
                for (var i = 0; i < argCount; ++i)
                {
                    args = DatumHelpers.cons(c.Result, args);
                    c = c.PopResult();
                }
                return function.Evaluate(c, args);
            }

            public override string ToString()
            {
                return string.Format("Invoke '{0}' with {1} args", function, argCount);
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var argArray = DatumHelpers.enumerate(args).ToArray();
            // Invoke elements in reverse so that resultant arg list can be constructed
            // in place.
            Array.Reverse(argArray);
            c = c.PushTask(new InvokeFunction(function, argArray.Length));
            return argArray.Aggregate(c, (current, arg) => current.Evaluate(env, arg));
        }
    }
}
