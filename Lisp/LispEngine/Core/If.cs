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
        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length != 3)
                throw error("If: Invalid syntax. Expected 3 arguments: condition, true-case, false-case. Got {0}", a.Length);
            var condition = a[0];
            var trueCase = a[1];
            var falseCase = a[2];
            return atom(false).Equals(evaluator.Evaluate(env, condition)) ? falseCase : trueCase;
        }
    }
}
