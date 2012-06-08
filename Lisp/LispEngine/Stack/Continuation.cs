using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
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

    static class Extensions
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
    }
}
