using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    internal sealed class Lambda : DatumHelpers, Macro
    {
        public static readonly Macro Instance = new Lambda();

        private Lambda()
        {
        }

        public Datum Expand(Evaluator evaluator, Environment env, Datum args)
        {
            var macroArgs = new List<Datum>(enumerate(args));
            if(macroArgs.Count != 2)
                throw new Exception("Invalid macro syntax for lambda");
            var arguments = BindingTypes.parse(macroArgs[0]);
            var body = macroArgs[1];

            // This is a "terminal" macro so it evaluates to
            // an atom which will then be evaluated to its value.
            return new Closure(env, arguments, body);
        }
    }
}
