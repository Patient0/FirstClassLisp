using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    abstract class BinaryFunction : Function
    {
        public Datum Evaluate(Datum args)
        {
            var argDatums = args.ToArray();
            if (argDatums.Length != 2)
                throw DatumHelpers.error("Exactly 2 arguments expected");
            return eval(argDatums[0], argDatums[1]);
        }

        protected abstract Datum eval(Datum arg1, Datum arg2);
    }
}
