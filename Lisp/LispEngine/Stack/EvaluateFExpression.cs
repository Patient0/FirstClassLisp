using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
{
    class EvaluateFExpression : Task
    {
        private readonly Datum args;
        private readonly Environment env;

        public EvaluateFExpression(Datum args, Environment env)
        {
            this.args = args;
            this.env = env;
        }

        public void Perform(EvaluatorStack stack)
        {
            var fexpression = Evaluator.toFExpression(stack.PopResult());
            fexpression.Evaluate(stack, env, args);
        }
    }
}
