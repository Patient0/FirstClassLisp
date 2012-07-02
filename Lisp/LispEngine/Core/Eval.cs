﻿using System;
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
        private static ErrorHandler makeErrorHandler(StackFunction f)
        {
            // Report the "message" from the exception to the Lisp
            // error handling function.
            return (c, ex) => f.Evaluate(c, DatumHelpers.atomList(ex.Message));
        }

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var argArray = args.ToArray();
            var expression = argArray[0];
            var environment = (Environment) argArray[1].CastObject();
            if(argArray.Length > 2)
                c = c.SetErrorHandler(makeErrorHandler((StackFunction) argArray[2]));
            return c.Evaluate(environment, expression);
        }

        public override string ToString()
        {
            return ",eval";
        }

        public static readonly StackFunction Instance = new Eval();
    }
}
