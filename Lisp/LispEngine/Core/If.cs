using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class If : AbstractFExpression
    {
        public static readonly FExpression Instance = new If();

        class CheckResult : Task
        {
            private readonly Environment env;
            private readonly int clause;
            private readonly IList<Datum> clauses;
            public CheckResult(Environment env, int clause, IList<Datum> clauses)
            {
                this.env = env;
                this.clause = clause;
                this.clauses = clauses;
            }

            public Continuation Perform(Continuation c)
            {
                var result = c.Result;
                c = c.PopResult();
                if(DatumHelpers.atom(true).Equals(result))
                    return c.Evaluate(env, clauses[clause+1]);
                var nextClause = clause + 2;
                // Result is false. If this was the last clause, then evaluate the default clause.
                if(nextClause >= clauses.Count - 1)
                {
                    var defaultClause = clauses[clauses.Count - 1];
                    return c.Evaluate(env, defaultClause);
                }
                // Result was false but there are more clauses.
                // Check the result of the next clause.
                c = c.PushTask(new CheckResult(env, nextClause, clauses));
                return c.Evaluate(env, clauses[nextClause]);
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var clauses = args.ToArray();
            c = c.PushTask(new CheckResult(env, 0, clauses));
            return c.Evaluate(env, clauses[0]);
        }
    }
}
