using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class DebugFunctions
    {
        private static Continuation asContinuation(Datum arg)
        {
            var cfunction = arg as CallCC.ContinuationFunction;
            if (cfunction == null)
                throw DatumHelpers.error("'{0}' is not a continuation", arg);
            return cfunction.Continuation;
        }

        public class TaskDescriptions : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var c = asContinuation(arg);
                var stack = DatumHelpers.nil;
                while (c.Task != null)
                {
                    stack = DatumHelpers.cons(c.Task.ToString().ToAtom(), stack);
                    c = c.PopTask();
                }
                return stack;
            }

            public override string ToString()
            {
                return ",task-descriptions";
            }
        }

        public class PendingResults : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var c = asContinuation(arg);
                var stack = DatumHelpers.nil;
                while(c.Result != null)
                {
                    stack = DatumHelpers.cons(c.Result, stack);
                    c = c.PopResult();
                }
                return stack;
            }
        }

        // We could probably redo the 'env' f-expression
        // to use this instead in combination with call/cc.
        public class GetEnv : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var c = asContinuation(arg);
                return c.Env.ToAtom();
            }
        }

        public class Throw : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var msg = (String)arg.CastObject();
                throw DatumHelpers.error(msg);
            }
        }

        public static Environment AddTo(Environment env)
        {
            return env.Extend("task-descriptions", new TaskDescriptions().ToStack())
                .Extend("execute-with-error-translator", ExecuteWithErrorTranslator.Instance)
                .Extend("pending-results", new PendingResults().ToStack())
                .Extend("throw", new Throw().ToStack())
                .Extend("get-env", new GetEnv().ToStack());
        }
    }
}
