using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class DebugFunctions : DelegateFunctions
    {
        private static Continuation asContinuation(Datum arg)
        {
            var cfunction = arg as CallCC.ContinuationFunction;
            if (cfunction == null)
                throw DatumHelpers.error("'{0}' is not a continuation", arg);
            return cfunction.Continuation;
        }

        private static Datum getEnvironments(Datum arg)
        {
            var c = asContinuation(arg);
            var stack = DatumHelpers.nil;
            while (c.Env != null)
            {
                stack = DatumHelpers.cons(c.Env.ToAtom(), stack);
                c = c.PopEnv();
            }
            return stack;
        }

        private static Datum getTaskDescriptions(Datum arg)
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

        private static Datum getPendingResults(Datum arg)
        {
            var c = asContinuation(arg);
            var stack = DatumHelpers.nil;
            while (c.Result != null)
            {
                stack = DatumHelpers.cons(c.Result, stack);
                c = c.PopResult();
            }
            return stack;
        }

        private static Datum getEnv(Datum arg)
        {
            var c = asContinuation(arg);
            return c.Env.ToAtom();            
        }

        private static Datum throwMsg(Datum arg)
        {
            var msg = (String)arg.CastObject();
            throw DatumHelpers.error(msg);            
        }

        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            return env.Define("task-descriptions", MakeDatumFunction(getTaskDescriptions, ",task-descriptions"))
                .Define("execute-with-error-translator", ExecuteWithErrorTranslator.Instance)
                .Define("env-stack", MakeDatumFunction(getEnvironments, ",env-stack"))
                .Define("pending-results", MakeDatumFunction(getPendingResults, ",pending-results"))
                .Define("throw", MakeDatumFunction(throwMsg, ",throw"))
                .Define("get-env", MakeDatumFunction(getEnv, ",get-env"));
        }
    }
}
