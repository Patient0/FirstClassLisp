using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
{
    class EvaluateTask : Task
    {
        private readonly Datum datum;
        private readonly Environment env;

        public EvaluateTask(Datum datum, Environment env)
        {
            this.datum = datum;
            this.env = env;
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
                stack.PushTask(new EvaluateTask(c.First, env));
                return;
            }
            // Anything else just evaluates to itself
            stack.PushResult(datum);
        }

        public string ToString()
        {
            return string.Format("Evaluate '{0}'", datum);
        }
    }
}
