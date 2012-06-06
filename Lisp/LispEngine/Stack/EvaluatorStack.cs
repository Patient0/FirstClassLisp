using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Stack
{
    public interface EvaluatorStack
    {
        void PushTask(Task task);
        Task PopTask();
        void PushResult(Datum d);
        Datum PopResult();
    }

    static class Extensions
    {
        public static void Evaluate(this EvaluatorStack s, Environment e, Datum expression)
        {
            s.PushTask(new EvaluateTask(e, expression));
        }

        private static StackFunction toStack(Function f)
        {
            return f as StackFunction ?? new StackFunctionAdapter(f);
        }

        public static void Invoke(this EvaluatorStack s, Function f, Datum args)
        {
            toStack(f).Evaluate(s, args);
        }
    }
}
