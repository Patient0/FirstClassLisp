using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    using ErrorHandler = Func<Continuation, Exception, Continuation>;

    /**
     * This "builtin" function provides a way to install a
     * Lisp function as the handler for what to do when a .Net exception
     * is thrown.
     */
    class ExecuteWithErrorTranslator : AbstractStackFunction
    {
        private static ErrorHandler makeErrorHandler(ErrorHandler oldErrorHandler, StackFunction f)
        {
            // Report the "message" from the exception to the Lisp
            // error handling function.
            // Ensure that the original error handler is in scope before evaluating the error function -
            // otherwise we end up in an infinite loop if there's an error in the error function itself.
            return (c, ex) => f.Evaluate(c.PopTask().SetErrorHandler(oldErrorHandler), DatumHelpers.compound(ex.Message.ToAtom(), CallCC.MakeContinuationFunction(c)));
        }

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var argArray = args.ToArray();
            if (argArray.Length != 2)
                throw DatumHelpers.error("Invalid syntax. ArgCount ({0}) != 2. Usage: (execute-with-error-handler <error-function> <fn>)", argArray.Length);
            var errorHandler = makeErrorHandler(c.ErrorHandler, (StackFunction)argArray[0]);
            var fn = (StackFunction)argArray[1];
            return fn.Evaluate(c.NewErrorHandler(errorHandler), DatumHelpers.compound());
        }

        public override string ToString()
        {
            return ",execute-with-error-translator";
        }

        public static readonly StackFunction Instance = new ExecuteWithErrorTranslator();
    }
}