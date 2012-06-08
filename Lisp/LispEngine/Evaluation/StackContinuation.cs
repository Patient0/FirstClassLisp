using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class StackContinuation : Continuation
    {
        private readonly IStack<Task> tasks;
        private readonly IStack<Datum> results;

        public static readonly Continuation Empty = new StackContinuation(Stack<Task>.Empty, Stack<Datum>.Empty);

        private StackContinuation(IStack<Task> tasks, IStack<Datum> results)
        {
            this.tasks = tasks;
            this.results = results;
        }

        public Continuation PushTask(Task task)
        {
            return new StackContinuation(tasks.Push(task), results);
        }

        public Continuation PopTask()
        {
            return new StackContinuation(tasks.Pop(), results);
        }

        public Continuation PushResult(Datum d)
        {
            return new StackContinuation(tasks, results.Push(d));
        }

        public Continuation PopResult()
        {
            return new StackContinuation(tasks, results.Pop());
        }

        public Task Task
        {
            get { return tasks.Peek(); }
        }

        public Datum Result
        {
            get { return results.Peek(); }
        }
    }
}
