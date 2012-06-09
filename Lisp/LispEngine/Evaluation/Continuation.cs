using System;
using System.IO;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
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

        public static string GetStackTrace(this Continuation c)
        {
            var sw = new StringWriter();
            while (c.Task != null)
            {
                sw.WriteLine("{0}", c.Task);
                c = c.PopTask();
            }
            sw.WriteLine("---TOP---");
            sw.Flush();
            return sw.ToString();
        }
    }
}
