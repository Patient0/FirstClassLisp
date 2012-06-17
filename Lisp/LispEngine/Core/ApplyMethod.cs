using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class ApplyMethod : AbstractFExpression
    {
        public static readonly FExpression Instance = new ApplyMethod();

        private ApplyMethod()
        {
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var datumArgs = args.ToArray();
            if (datumArgs.Length < 2)
                throw c.error("ApplyMethod expects at least 2 arguments. {0} passed", datumArgs.Length);
            var symbol = datumArgs[0] as Symbol;
            if (symbol == null)
                throw c.error("'{0}' is not a symbol", datumArgs[0]);
            var obj = DatumHelpers.castAtom(datumArgs[1]);
            var mi = obj.GetType().GetMethod(symbol.Identifier);
            var result = mi.Invoke(obj, datumArgs.Skip(2).Select(DatumHelpers.castAtom).ToArray());
            return c.PushResult(new Atom(result));
        }
    }
}
