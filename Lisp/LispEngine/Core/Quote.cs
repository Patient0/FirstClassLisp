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
    class Quote : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new Quote();

        private static Datum evaluate(Datum args)
        {
            var argList = enumerate(args).ToArray();
            if (argList.Length != 1)
                throw error("invalid syntax '{0}'", args);
            return argList[0];            
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            return evaluate(args);
        }

        public void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            evaluator.PushResult(evaluate(args));
        }
    }
}
