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
        private class ContinuationFunction : AbstractStackFunction
        {
            private readonly Continuation c;
            public ContinuationFunction(Continuation c)
            {
                this.c = c;
            }
            public override Continuation Evaluate(Continuation oldContinuation, Datum args)
            {
                // Replace the old continuation with the new continuation - but pass in the
                // supplied argument as the 'return value' of the new continuation.
                var returnValue = DatumHelpers.enumerate(args).ToArray()[0];
                return c.PushResult(returnValue);
            }
        }

        public static readonly StackFunction Instance = new CallCC();

        public override Continuation Evaluate(Continuation c, Datum args)
        {
            var argArray = DatumHelpers.enumerate(args).ToArray();
            if(argArray.Length != 1)
                throw DatumHelpers.error("call/cc: expect a single function as an argument. Got {0}", argArray.Length);
            var arg = argArray[0];
            var function = arg as StackFunction;
            if(function == null)
                throw DatumHelpers.error("call/cc: {0} must be a function", arg);
            return function.Evaluate(c, DatumHelpers.compound(new ContinuationFunction(c)));
        }
    }
}
