using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using ErrorHandler = Func<Continuation, Exception, Continuation>;

    class StackContinuation : Continuation
    {
        private readonly IStack<Task> tasks;
        private readonly IStack<Datum> results;
        private readonly ErrorHandler errorHandler;

        public static readonly Continuation Empty = new StackContinuation(Stack<Task>.Empty, Stack<Datum>.Empty, null);

        private StackContinuation(IStack<Task> tasks, IStack<Datum> results, ErrorHandler errorHandler)
        {
            this.tasks = tasks;
            this.results = results;
            this.errorHandler = errorHandler;
        }

        public Continuation PushTask(Task task)
        {
            return new StackContinuation(tasks.Push(task), results, errorHandler);
        }

        public Continuation PopTask()
        {
            return new StackContinuation(tasks.Pop(), results, errorHandler);
        }

        public Continuation PushResult(Datum d)
        {
            return new StackContinuation(tasks, results.Push(d), errorHandler);
        }

        public Continuation PopResult()
        {
            return new StackContinuation(tasks, results.Pop(), errorHandler);
        }

        public Task Task
        {
            get { return tasks.Peek(); }
        }

        public Datum Result
        {
            get { return results.Peek(); }
        }

        public ErrorHandler ErrorHandler
        {
            get
            {
                return errorHandler;
            }
        }

        public Continuation SetErrorHandler(ErrorHandler newHandler)
        {
            return new StackContinuation(tasks, results, newHandler);
        }
    }
}
