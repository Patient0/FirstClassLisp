using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Define : AbstractFExpression
    {
        public static readonly FExpression Instance = new Define();

        public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
        {
            var argList = args.ToArray();
            if (argList.Length != 2)
                throw c.error("Expected 2 arguments: (define <symbol> <expression>). Got {0} instead", argList.Length);
            var name = argList[0].CastSymbol();
            var expression = argList[1];
            c = c.PushTask(
                tc => { env.Define(name, tc.Result);
                        return tc;},
                "define '{0}'", name);
            return c.Evaluate(env, expression);
        }
    }
}
