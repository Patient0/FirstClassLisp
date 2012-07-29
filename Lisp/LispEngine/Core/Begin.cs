using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Begin : AbstractFExpression
    {
        public static readonly FExpression Instance = new Begin();

        private static Continuation popResult(Continuation c)
        {
            return c.PopResult();
        }

        public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
        {
            var argList = args.ToArray();
            if (argList.Length < 1)
                throw c.error("Expected at least 1 expression for begin. Got none.");
            // Scope any local definitions.
            var localEnv = env.NewFrame();
            var remaining = argList.Reverse().ToArray();
            for (var i = 0; i < remaining.Length; ++i)
            {
                if (i > 0)
                    c = c.PushTask(popResult, "Discard result");
                c = c.Evaluate(localEnv, remaining[i]);
            }
            return c;
        }

        public override string ToString()
        {
            return ",begin";
        }
    }
}
