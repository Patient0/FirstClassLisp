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

        private static Func<Datum, object> CreateEvaluator(Continuation c, Environment env)
        {
            Evaluator e = new Evaluator();
            return datum =>
                {
                    var c1 = c.Evaluate(env, datum);
                    var d1 = e.Evaluate(c1);

                    // TEMP: treat quoted symbols as string literals, until we have proper string expressions
                    var symbol = d1 as Symbol;
                    if (symbol == null)
                        return DatumHelpers.castAtom(d1);
                    else
                        return symbol.Identifier;
                };
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var datumArgs = args.ToArray();
            if (datumArgs.Length < 2)
                throw c.error("ApplyMethod expects at least 2 arguments. {0} passed", datumArgs.Length);
            var symbol = datumArgs[0] as Symbol;
            if (symbol == null)
                throw c.error("'{0}' is not a symbol", datumArgs[0]);
            var evaluator = CreateEvaluator(c, env);
            var obj = evaluator(datumArgs[1]);
            var type = obj.GetType();
            var mi = type.GetMethods().Where(m => m.Name == symbol.Identifier).FirstOrDefault();
            if (mi == null)
                throw c.error("No method {0} on {1}", symbol.Identifier, type.Name);
            var result = mi.Invoke(obj, datumArgs.Skip(2).Select(evaluator).ToArray());
            return c.PushResult(new Atom(result));
        }
    }
}
