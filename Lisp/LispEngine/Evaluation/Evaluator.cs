using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class Evaluator
    {
        public Datum Evaluate(Continuation c)
        {
            while (c.Task != null)
            {
                try
                {
                    c = c.Task.Perform(c.PopTask());
                }
                catch(Exception ex)
                {
                    if (c.ErrorHandler == null)
                        throw;
                    c = c.ErrorHandler(c.PopTask(), ex);
                }
            }
            return c.Result;
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            var c = Continuation.Empty
                .PushTask(null)
                .PushResult(null)
                .Evaluate(env, datum);
            return Evaluate(c);
        }
    }
}
