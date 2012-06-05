using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Stack
{
    public class StackEvaluator
    {
        private class S : EvaluatorStack
        {
            private readonly Stack<Task> tasks = new Stack<Task>();
            private readonly Stack<Datum> results = new Stack<Datum>();

            public void PushTask(Task task)
            {
                tasks.Push(task);
            }

            public Task PopTask()
            {
                return tasks.Pop();
            }

            public void PushResult(Datum d)
            {
                results.Push(d);
            }

            public Datum PopResult()
            {
                return results.Pop();
            }
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            var stack = new S();
            stack.PushTask(null);
            stack.PushTask(new EvaluateTask(datum, env));
            Task next;
            while ((next = stack.PopTask()) != null)
                next.Perform(stack);
            return stack.PopResult();
        }
    }
}
