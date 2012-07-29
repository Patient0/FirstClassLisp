using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Set : AbstractFExpression
    {
        public static readonly FExpression Instance = new Set();

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            var argList = args.ToArray();
            if (argList.Length != 2)
                throw c.error("Expected 2 arguments: (set! <symbol> <expression>). Got {0} instead", argList.Length);
            var name = argList[0].CastSymbol();
            var expression = argList[1];
            c = c.PushTask(s =>
                               {
                                   env.Set(name, s.Result);
                                   return s;
                               }, "set! '{0}'", name);
            return c.Evaluate(env, expression);
        }

        public override string ToString()
        {
            return ",set!";
        }
    }
}
