using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Define : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new Define();
        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            var argList = enumerate(args).ToArray();
            if (argList.Length < 2)
                throw error("Expected at least 2 arguments for define. Got {0}", argList.Length);
            var name = argList[0] as Symbol;
            if (name == null)
                throw error("Invalid define syntax. '{0}' should be a symbol", argList[0]);
            Datum value = null;
            for(var i = 1; i < argList.Length; ++i)
            {
                // Allow multiple statements inside a define. This allows
                // use to define nested symbols without polluting global
                // namespace.
                value = evaluator.Evaluate(env, argList[i]);
            }
            // Symbol is defined to be the last one
            env.Define(name.Identifier, value);
            // Have the whole expression also evaluate to the last value.
            return value;
        }
    }
}
