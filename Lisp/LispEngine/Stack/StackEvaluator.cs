using System;
using System.Collections.Generic;
using LispEngine.Datums;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Stack
{
    public class StackEvaluator
    {
        // TODO: Make this immutable. Then we can save/resume continuations
        private class S : Continuation
        {
            private readonly Stack<Task> tasks = new Stack<Task>();
            private readonly Stack<Datum> results = new Stack<Datum>();

            public Continuation PushTask(Task task)
            {
                tasks.Push(task);
                return this;
            }

            public Task Task
            {
                get { return tasks.Peek(); }
            }

            public Datum Result
            {
                get { return results.Peek(); }
            }

            public Continuation PopTask()
            {
                tasks.Pop();
                return this;
            }

            public Continuation PushResult(Datum d)
            {
                results.Push(d);
                return this;
            }

            public Continuation PopResult()
            {
                results.Pop();
                return this;
            }
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            Continuation c = new S();
            c = c.PushTask(null);
            c = c.PushResult(null);
            c = c.Evaluate(env, datum);
            while(c.Task != null)
            {
                var task = c.Task;
                c = task.Perform(c.PopTask());
            }
            c = c.PopTask();
            var result = c.Result;
            c = c.PopResult();
            if(c.Result != null)
                throw new Exception(string.Format("Additional '{0}' on result stack", c.Result));
            return result;
        }
    }
}
