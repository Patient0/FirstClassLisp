using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class Evaluator
    {
        public Evaluator()
        {
            Steps = 0;
        }

        public Datum Evaluate(Continuation c)
        {
            while (c.Task != null)
            {
                try
                {
                    c = c.Task.Perform(c.PopTask());
                    ++Steps;
                }
                catch(Exception ex)
                {
                    if (c.ErrorHandler == null)
                        throw;
                    c = c.ErrorHandler(c, ex);
                }
            }
            return c.Result;
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            Steps = 0;
            var c = Continuation.Empty
                .PushTask(null)
                .PushResult(null)
                .Evaluate(env, datum);
            return Evaluate(c);
        }

        public int Steps { get; private set; }
    }
}
