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
            private readonly IList<Datum> clauses;
            public CheckResult(Environment env, IList<Datum> clauses)
            {
                this.env = env;
                this.clauses = clauses;
            }

            public Continuation Perform(Continuation c)
            {
                var result = c.Result;
                c = c.PopResult();
                if (DatumHelpers.atom(true).Equals(result))
                    return c.Evaluate(env, clauses[1]);
                return c.Evaluate(env, clauses[2]);
            }
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var clauses = args.ToArray();
            if (clauses.Length != 3)
                throw c.error("Invalid if syntax: Expected (if <condition> <true-case> <false-case>). Got '{0}'", args);
            c = c.PushTask(new CheckResult(env, clauses));
            return c.Evaluate(env, clauses[0]);
        }
    }
}
