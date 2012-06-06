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
            stack.PushResult(null);
            stack.Evaluate(env, datum);
            Task next;
            while ((next = stack.PopTask()) != null)
                next.Perform(stack);
            
            var result = stack.PopResult();
            var additional = stack.PopResult();
            if(additional != null)
            {
                throw new Exception(string.Format("Additional '{0}' on result stack", additional));
            }
            return result;
        }
    }
}
