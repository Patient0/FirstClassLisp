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

        public static readonly Continuation Empty = new Continuation(Stack<Environment>.Empty, Stack<Task>.Empty, Stack<Datum>.Empty, null);

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

        class RestoreErrorHandler : Task
        {
            private readonly ErrorHandler previous;
            public RestoreErrorHandler(ErrorHandler previous)
            {
                this.previous = previous;
            }
            public Continuation Perform(Continuation c)
            {
                return c.SetErrorHandler(previous);
            }
            public override string ToString()
            {
                return string.Format("Restore error handler '{0}'", previous);
            }
        }

        public Continuation NewErrorHandler(ErrorHandler errorHandler)
        {
            // Set the current error handler to something new, but also
            // remember to restore the old error handler once we get past this
            // point.
            return PushTask(new RestoreErrorHandler(ErrorHandler)).SetErrorHandler(errorHandler);
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
    }
}
