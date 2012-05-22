using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    internal sealed class Lambda : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new Lambda();

        private Lambda()
        {
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            var macroArgs = new List<Datum>(enumerate(args));
            if(macroArgs.Count != 2)
                throw new Exception("Invalid macro syntax for lambda");
            var closureArgs = macroArgs[0];
            var body = macroArgs[1];

            return new Closure(env, closureArgs, body);
        }
    }
}
