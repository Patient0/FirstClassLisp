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
    class Quote : AbstractFExpression
    {
        public static readonly FExpression Instance = new Quote();

        private static Datum evaluate(Datum args)
        {
            var argList = DatumHelpers.enumerate(args).ToArray();
            if (argList.Length != 1)
                throw DatumHelpers.error("invalid syntax '{0}'", args);
            return argList[0];            
        }

        public override void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            evaluator.PushResult(evaluate(args));
        }
    }
}
