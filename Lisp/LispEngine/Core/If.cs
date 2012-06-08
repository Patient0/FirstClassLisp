using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
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

            public void Perform(EvaluatorStack stack)
            {
                var result = stack.PopResult();
                if(DatumHelpers.atom(true).Equals(result))
                {
                    stack.Evaluate(env, clauses[clause+1]);
                    return;
                }
                var nextClause = clause + 2;
                // Result is false. If this was the last clause, then evaluate the default clause.
                if(nextClause >= clauses.Count)
                {
                    var defaultClause = clauses[clauses.Count - 1];
                    stack.Evaluate(env, defaultClause);
                } 
                else
                {
                    // Result was false but there are more clauses.
                    // Check the result of the next clause.
                    stack.PushTask(new CheckResult(env, nextClause, clauses));
                    stack.Evaluate(env, clauses[nextClause]);
                }
            }
        }

        public override void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            var clauses = DatumHelpers.enumerate(args).ToArray();
            evaluator.PushTask(new CheckResult(env, 0, clauses));
            evaluator.Evaluate(env, clauses[0]);
        }
    }
}
