using System;
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

        public Environment Env
        {
            get { return env; }
        }

        class Visitor : AbstractVisitor<Continuation>
        {
            private readonly Environment env;
            private readonly Continuation c;
            public Visitor(Continuation c, Environment env)
            {
                this.c = c;
                this.env = env;
            }
            public override Continuation visit(Pair p)
            {
                return c
                    .PushTask(new EvaluateFExpression(p.Second, env))
                    .Evaluate(env, p.First);
            }
            public override Continuation visit(Symbol s)
            {
                return c.PushResult(env.Lookup(s.Identifier));
            }
            public override Continuation defaultCase(Datum d)
            {
                return c.PushResult(d);
            }
        }

        public Continuation Perform(Continuation c)
        {
            return datum.accept(new Visitor(c, env));
        }

        public override string ToString()
        {
            return string.Format("Evaluate '{0}'", datum);
        }
    }
}
