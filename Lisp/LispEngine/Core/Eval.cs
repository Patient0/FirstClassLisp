using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class Eval : AbstractStackFunction
    {
        public override Continuation Evaluate(Continuation s, Datum args)
        {
            var argArray = args.ToArray();
            var expression = argArray[0];
            var environment = (Environment) argArray[1].CastObject();
            return s.Evaluate(environment, expression);
        }

        public override string ToString()
        {
            return ",eval";
        }

        public static readonly StackFunction Instance = new Eval();
    }
}
