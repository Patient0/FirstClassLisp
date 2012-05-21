using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    /**
     * Turns a function into a macro
     */
    class UserMacro : DatumHelpers, Function
    {
        public static readonly Function Instance = new UserMacro();
        class MacroClosure : Macro
        {
            private readonly Function argFunction;

            public MacroClosure(Function argFunction)
            {
                this.argFunction = argFunction;
            }

            public Datum Expand(Evaluator evaluator, Environment env, Datum args)
            {
                return argFunction.Evaluate(evaluator, args);
            }
        }

        public Datum Evaluate(Evaluator evaluator, Datum args)
        {
            var a = enumerate(args).ToArray();
            if (a.Length != 1)
                // TODO: Move this error boiler plate into DatumHelpers
                throw error("Incorrect arguments. Expected {0}, got {1}", 1, a.Length);
            var function = a[0] as Function;
            if (function == null)
                throw error("Expected function argument");
            return new MacroClosure(function);
        }
    }
}
