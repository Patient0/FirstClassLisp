using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Env : AbstractFExpression
    {
        public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
        {
            return c.PushResult(env.ToAtom());
        }

        public override string ToString()
        {
            return ",env";
        }

        public static FExpression Instance = new Env();
    }
}
