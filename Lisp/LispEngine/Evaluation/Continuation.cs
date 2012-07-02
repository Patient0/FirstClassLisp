using System;
using System.IO;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using ErrorHandler = Func<Continuation, Exception, Continuation>;

    public interface Continuation
    {
        Continuation PushTask(Task task);
        Continuation PopTask();
        Continuation PushResult(Datum d);
        Continuation PopResult();

        // The current task
        Task Task { get; }
        // The current result
        Datum Result { get; }

        ErrorHandler ErrorHandler { get; }
        Continuation SetErrorHandler(ErrorHandler errorHandler);
    }

    static class ContinuationExtensions
    {
        public static Continuation Evaluate(this Continuation s, Environment e, Datum expression)
        {
            return s.PushTask(new EvaluateTask(e, expression));
        }

        private static StackFunction toStack(Function f)
        {
            return f as StackFunction ?? new StackFunctionAdapter(f);
        }

        public static Continuation Invoke(this Continuation s, Function f, Datum args)
        {
            return toStack(f).Evaluate(s, args);
        }

        public static EvaluationException error(this Continuation c, string msg, params object[] args)
        {
            return error(c, null, msg, args);
        }

        public static EvaluationException error(this Continuation c, Exception cause, string msg, params object[] args)
        {
            return new EvaluationException(string.Format(msg, args), c, cause);
        }

        private static void dumpTasks(Continuation c, TextWriter sw)
        {
            sw.WriteLine("Tasks:");
            while (c.Task != null)
            {
                sw.WriteLine("{0}", c.Task);
                c = c.PopTask();
            }
        }

        private static void dumpResults(Continuation c, TextWriter sw)
        {
            sw.WriteLine("Results:");
            while (c.Result != null)
            {
                sw.WriteLine("{0}", c.Result);
                c = c.PopResult();
            }
        }

        public static string GetStackTrace(this Continuation c)
        {
            var sw = new StringWriter();
            dumpTasks(c, sw);
            dumpResults(c, sw);
            sw.Flush();
            return sw.ToString();
        }

        // The following are useful for debugging.
        public static IEnvironment GetEnvironment(this Continuation c)
        {
            EvaluateTask t = null;
            while(c.Task != null && (t = c.Task as EvaluateTask) == null)
                c = c.PopTask();
            return t == null ? null : t.Env;
        }

        public static Datum Lookup(this Continuation c, string name)
        {
            var env = GetEnvironment(c);
            if (env == null)
                return null;

            Datum datum;
            if (env.TryLookup(name, out datum))
                return datum;

            return DatumHelpers.atom("undefined");
        }
    }
}
