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
                var args = new Datum[argCount];
                for (var i = 0; i < argCount; ++i)
                {
                    args[i] = c.Result;
                    c = c.PopResult();
                }
                return function.Evaluate(c, DatumHelpers.compound(args));
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var argArray = DatumHelpers.enumerate(args).ToArray();
            c = c.PushTask(new InvokeFunction(function, argArray.Length));
            return argArray.Aggregate(c, (current, arg) => current.Evaluate(env, arg));
        }
    }
}
