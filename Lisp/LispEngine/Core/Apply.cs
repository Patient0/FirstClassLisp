using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Apply : DatumHelpers, Function
    {
        public static readonly Function Instance = new Apply();

        private Apply()
        {
        }

        public Datum Evaluate(Evaluator evaluator, Datum args)
        {
            var datumArgs = enumerate(args).ToArray();
            if (datumArgs.Length != 2)
                throw new Exception(string.Format("Apply expects 2 arguments. {0} passed", datumArgs.Length));
            var function = datumArgs[0] as Function;
            if (function == null)
                throw new Exception(string.Format("'{0}' is not a function", datumArgs[0]));
            return function.Evaluate(evaluator, datumArgs[1]);
        }
    }
}
