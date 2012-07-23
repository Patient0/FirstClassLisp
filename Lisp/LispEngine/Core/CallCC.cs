using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class CallCC : AbstractStackFunction
    {
        public class ContinuationFunction : AbstractStackFunction
        {
            private readonly Continuation c;
            public ContinuationFunction(Continuation c)
            {
                this.c = c;
            }

            public Continuation Continuation
            {
                get { return c; }
            }

            public override Continuation Evaluate(Continuation oldContinuation, Datum args)
            {
                // Replace the old continuation with the new continuation - but pass in the
                // supplied argument as the 'return value' of the new continuation.
                var argArray = args.ToArray();
                // We allow calling a "continuation" with 0 args. Such a continuation
                // only arises from the error function.
                // TODO: we should differentiate the two with an "expected args" member which we can error check.
                if (argArray.Length == 0)
                    return c;
                return c.PushResult(argArray[0]);
            }
        }

        public static StackFunction MakeContinuationFunction(Continuation c)
        {
            return new ContinuationFunction(c);
        }

        public static readonly StackFunction Instance = new CallCC();

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var argArray = args.ToArray();
            if(argArray.Length != 1)
                throw DatumHelpers.error("call-cc: expect a single function as an argument. Got {0}", argArray.Length);
            var arg = argArray[0];
            var function = arg as StackFunction;
            if(function == null)
                throw DatumHelpers.error("call-cc: {0} must be a function", arg);
            return function.Evaluate(c, DatumHelpers.compound(MakeContinuationFunction(c)));
        }
    }
}
