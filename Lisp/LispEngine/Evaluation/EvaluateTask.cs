using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class EvaluateTask : Task
    {
        private readonly Datum datum;

        public EvaluateTask(Datum datum)
        {
            this.datum = datum;
        }

        class Visitor : AbstractVisitor<Continuation>
        {
            private readonly LexicalEnvironment env;
            private readonly Continuation c;
            public Visitor(Continuation c, LexicalEnvironment env)
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
                return c.PushResult(env.Lookup(s));
            }
            public override Continuation defaultCase(Datum d)
            {
                return c.PushResult(d);
            }
        }

        public Continuation Perform(Continuation c)
        {
            return datum.accept(new Visitor(c.PopEnv(), c.Env));
        }

        private static object getLocation(Datum d)
        {
            var pair = d as Pair;
            return pair == null ? null : pair.Location;
        }

        public override string ToString()
        {
            return string.Format("Evaluate '{0}' ({1})", datum, getLocation(datum));
        }
    }
}
