using LispEngine.Datums;

namespace LispEngine.Evaluation
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

        public Continuation Perform(Continuation c)
        {
            var s = datum as Symbol;
            if (s != null)
                return c.PushResult(env.Lookup(s.Identifier));
            var p = datum as Pair;
            if (p != null)
            {
                c = c.PushTask(new EvaluateFExpression(p.Second, env));
                return c.Evaluate(env, p.First);
            }
            // Anything else just evaluates to itself
            return c.PushResult(datum);
        }

        public override string ToString()
        {
            return string.Format("Evaluate '{0}'", datum);
        }
    }
}
