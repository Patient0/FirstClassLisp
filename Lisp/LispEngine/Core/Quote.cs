using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Quote : FExpression
    {
        public static readonly FExpression Instance = new Quote();
        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            return args;
        }
    }
}
