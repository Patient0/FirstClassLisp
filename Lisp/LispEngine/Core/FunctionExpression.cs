using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class FunctionExpression : DatumHelpers, FExpression
    {
        private readonly Function function;

        private static Datum evaluateArgs(Evaluator evaluator, Environment env, Datum datum)
        {
            return compound(enumerate(datum).Select(f => evaluator.Evaluate(env, f)).ToArray());
        }

        public FunctionExpression(Function function)
        {
            this.function = function;
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            return function.Evaluate(evaluator, evaluateArgs(evaluator, env, args));
        }
    }
}
