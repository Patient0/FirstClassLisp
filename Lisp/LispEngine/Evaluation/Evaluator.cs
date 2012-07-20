using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class Evaluator
    {
        private static Datum Evaluate(Statistics.Statistic<int> steps, Continuation c)
        {
            while (c.Task != null)
            {
                try
                {
                    c = c.Task.Perform(c.PopTask());
                    ++steps.Value;
                }
                catch(Exception ex)
                {
                    c = c.ErrorHandler(c, ex);
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
            return Evaluate(Statistics.Get(env).GetCounter("steps"), c);
        }
    }
}
