using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
{
    class EvaluateFExpression : Task
    {
        private readonly Datum args;
        private readonly Environment env;

        private static FExpression toFExpression(Datum d)
        {
            var fexpr = d as FExpression;
            if (fexpr != null)
                return fexpr;
            var function = d as StackFunction;
            if (function != null)
                return new FunctionExpression(function);
            throw DatumHelpers.error("'{0}' is not callable", d);
        }

        public EvaluateFExpression(Datum args, Environment env)
        {
            this.args = args;
            this.env = env;
        }

        public void Perform(EvaluatorStack stack)
        {
            var fexpression = toFExpression(stack.PopResult());
            fexpression.Evaluate(stack, env, args);
        }
    }
}
