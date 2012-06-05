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
    class If : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new If();
        private static Datum choose(Evaluator evaluator, Environment env, IList<Datum> clauses)
        {
            var clause = 0;
            while(atom(false).Equals(evaluator.Evaluate(env, clauses[clause])) && clause < clauses.Count - 1)
            {
                clause += 2;
            }
            return clause < clauses.Count - 1 ? clauses[clause + 1] : clauses[clauses.Count - 1];
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length % 2 == 0)
                throw error("If: Invalid syntax. Expected an odd number of arguments: (condition) (body) ... (default). Got {0}", a.Length);
            var branch = choose(evaluator, env, a);
            return evaluator.Evaluate(env, branch);
        }

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
                if(atom(true).Equals(result))
                {
                    stack.PushTask(new EvaluateTask(clauses[clause+1], env));
                    return;
                }
                var nextClause = clause + 2;
                // Result is false. If this was the last clause, then evaluate the default clause.
                if(nextClause >= clauses.Count)
                {
                    var defaultClause = clauses[clauses.Count - 1];
                    stack.PushTask(new EvaluateTask(defaultClause, env));
                } 
                else
                {
                    // Result was false but there are more clauses.
                    // Check the result of the next clause.
                    stack.PushTask(new CheckResult(env, nextClause, clauses));
                    stack.PushTask(new EvaluateTask(clauses[nextClause], env));
                }
            }
        }

        public void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            var clauses = enumerate(args).ToArray();
            evaluator.PushTask(new CheckResult(env, 0, clauses));
            evaluator.PushTask(new EvaluateTask(clauses[0], env));
        }
    }
}
