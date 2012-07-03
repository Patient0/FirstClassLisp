using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    using ErrorHandler = Func<Continuation, Exception, Continuation>;

    class Eval : AbstractStackFunction
    {
        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var argArray = args.ToArray();
            var expression = argArray[0];
            var environment = (Environment) argArray[1].CastObject();
            return c.Evaluate(environment, expression);
        }

        public override string ToString()
        {
            return ",eval";
        }

        public static readonly StackFunction Instance = new Eval();
    }
}
