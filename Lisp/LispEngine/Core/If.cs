using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class If : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new If();
        private static Datum choose(Evaluator evaluator, Environment env, Datum[] clauses)
        {
            var clause = 0;
            while(atom(false).Equals(evaluator.Evaluate(env, clauses[clause])) && clause < clauses.Length - 1)
            {
                clause += 2;
            }
            return clause < clauses.Length - 1 ? clauses[clause + 1] : clauses[clauses.Length - 1];
        }
        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length % 2 == 0)
                throw error("If: Invalid syntax. Expected an odd number of arguments: (condition) (body) ... (default). Got {0}", a.Length);
            var branch = choose(evaluator, env, a);
            return evaluator.Evaluate(env, branch);
        }
    }
}
