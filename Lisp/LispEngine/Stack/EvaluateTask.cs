using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
{
    class EvaluateTask : Task
    {
        private readonly Environment env;
        private readonly Datum datum;

        public EvaluateTask(Environment env, Datum datum)
        {
            this.env = env;
            this.datum = datum;
        }

        public void Perform(EvaluatorStack stack)
        {
            var s = datum as Symbol;
            if (s != null)
            {
                stack.PushResult(env.Lookup(s.Identifier));
                return;
            }
            var c = datum as Pair;
            if (c != null)
            {
                stack.PushTask(new EvaluateFExpression(c.Second, env));
                stack.Evaluate(env, c.First);
                return;
            }
            // Anything else just evaluates to itself
            stack.PushResult(datum);
        }

        public override string ToString()
        {
            return string.Format("Evaluate '{0}'", datum);
        }
    }
}
