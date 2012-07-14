using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    abstract class UnaryFunction : Function
    {
        public static Datum GetSingle(Datum args)
        {
            var argArray = args.ToArray();
            if (argArray.Length != 1)
                throw DatumHelpers.error("Expected a single argument. Got {0}", argArray.Length);
            return argArray[0];
            
        }
        public Datum Evaluate(Datum args)
        {
            return eval(GetSingle(args));
        }

        protected abstract Datum eval(Datum arg);
    }
}
