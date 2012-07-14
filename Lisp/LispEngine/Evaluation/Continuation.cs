using System;
using System.IO;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    using ErrorHandler = Func<Continuation, Exception, Continuation>;

    public class Continuation
    {
        private readonly IStack<Environment> envs;
        private readonly IStack<Task> tasks;
        private readonly IStack<Datum> results;
        private readonly ErrorHandler errorHandler;

        public static Continuation Unhandled(Continuation c, Exception ex)
        {
            throw new EvaluationException(c, ex);
        }

        public static readonly Continuation Empty = new Continuation(Stack<Environment>.Empty, Stack<Task>.Empty, Stack<Datum>.Empty, Unhandled);

        private Continuation(IStack<Environment> envs, IStack<Task> tasks, IStack<Datum> results, ErrorHandler errorHandler)
        {
            this.envs = envs;
            this.tasks = tasks;
            this.results = results;
            this.errorHandler = errorHandler;
        }

        private Continuation create(IStack<Environment> newEnvs, IStack<Task> newTasks, IStack<Datum> newResults)
        {
            return new Continuation(newEnvs, newTasks, newResults, errorHandler);
        }

        private Continuation SetTasks(IStack<Task> newTasks)
        {
            return create(envs, newTasks, results);
        }

        private Continuation SetEnvs(IStack<Environment> newEnvs)
        {
            return create(newEnvs, tasks, results);
        }

        private Continuation SetResults(IStack<Datum> newResults)
        {
            return create(envs, tasks, newResults);
        }

        public Continuation SetErrorHandler(ErrorHandler newHandler)
        {
            return new Continuation(envs, tasks, results, newHandler);
        }

        public Continuation PushEnv(Environment env)
        {
            return SetEnvs(envs.Push(env));
        }

        public Continuation PopEnv()
        {
            return SetEnvs(envs.Pop());
        }

        public Continuation PushTask(Task task)
        {
            return SetTasks(tasks.Push(task));
        }

        public Continuation PushTask(Func<Continuation, Continuation> taskDelegate, string fmt, params object[] args)
        {
            return PushTask(new DelegateTask(taskDelegate, fmt, args));
        }

        public Continuation PopTask()
        {
            return SetTasks(tasks.Pop());
        }

        public Continuation PushResult(Datum d)
        {
            return SetResults(results.Push(d));
        }

        public Continuation PopResult()
        {
            return SetResults(results.Pop());
        }

        public Task Task
        {
            get { return tasks.Peek(); }
        }

        public Datum Result
        {
            get { return results.Peek(); }
        }

        public Environment Env
        {
            get { return envs.Peek(); }
        }

        public ErrorHandler ErrorHandler
        {
            get { return errorHandler; }
        }

        private static StackFunction toStack(Function f)
        {
            return f as StackFunction ?? new StackFunctionAdapter(f);
        }

        public Continuation Invoke(Function f, Datum args)
        {
            return toStack(f).Evaluate(this, args);
        }

        public Continuation Evaluate(Environment e, Datum expression)
        {
            return PushEnv(e).PushTask(new EvaluateTask(expression));
        }

        public Continuation NewErrorHandler(ErrorHandler errorHandler)
        {
            // Set the current error handler to something new, but also
            // remember to restore the old error handler once we get past this
            // point.
            // We can't just keep a 'stack' of error handlers in case the error handling
            // function itself doesn't escape by invoking a continuation.
            return PushTask(c => c.SetErrorHandler(ErrorHandler), "RestoreErrorHandler").SetErrorHandler(errorHandler);
        }
    }

    static class ContinuationExtensions
    {
        public static Exception error(this Continuation c, string msg, params object[] args)
        {
            return error(c, null, msg, args);
        }

        public static Exception error(this Continuation c, Exception cause, string msg, params object[] args)
        {
            return new Exception(string.Format(msg, args), cause);
        }
    }
}
