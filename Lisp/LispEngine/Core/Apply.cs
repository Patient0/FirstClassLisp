using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Apply : AbstractStackFunction
    {
        public static readonly StackFunction Instance = new Apply();

        private Apply()
        {
        }

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var datumArgs = args.ToArray();
            if (datumArgs.Length != 2)
                throw c.error("Apply expects 2 arguments. {0} passed", datumArgs.Length);
            var function = datumArgs[0] as StackFunction;
            if (function == null)
                throw c.error("'{0}' is not a function", datumArgs[0]);
            return function.Evaluate(c, datumArgs[1]);
        }
    }
}
