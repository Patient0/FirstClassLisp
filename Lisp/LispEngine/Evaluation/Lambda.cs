using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    sealed class Lambda : DatumHelpers, Macro
    {
        public static readonly Macro Instance = new Lambda();

        public Datum Expand(Evaluator evaluator, Environment env, Datum args)
        {
            var macroArgs = new List<Datum>(enumerate(args));
            if(macroArgs.Count != 2)
                throw new Exception("Invalid macro syntax for lambda");

            // TODO: Allow (lambda x x) syntax which binds x to all the arguments
            // TODO: Allow (lambda (x . y) y) syntax which binds to pair
            var argSymbols = enumerate(macroArgs[0]).Select(datum => datum as Symbol);
            if(argSymbols.Any(symbol => symbol == null))
                throw new Exception("Invalid arg syntax in lambda");

            var argNames = argSymbols.Select(s => s.Identifier);

            var body = macroArgs[1];

            // This is a "terminal" macro so it evaluates to
            // an atom which will then be evaluated to its value.
            return atom(new Closure(evaluator, env, argNames, body));
        }
    }
}
