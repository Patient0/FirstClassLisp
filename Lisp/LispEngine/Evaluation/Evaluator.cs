using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class Evaluator
    {
        private static Datum Evaluate(Continuation c)
        {
            while (c.Task != null)
            {
                try
                {
                    c = c.Task.Perform(c.PopTask());
                    c.Statistics.Steps++;
                }
                catch (Exception ex)
                {
                    c = c.ErrorHandler(c, ex);
                }
            }
            return c.Result;
        }

        public Datum Evaluate(Environment env, Datum datum)
        {
            return Evaluate(new Statistics(), env, datum);
        }

        public Datum Evaluate(Statistics statistics, Environment env, Datum datum)
        {
            var c = Continuation.Create(statistics)
                .PushTask(null)
                .PushResult(null)
                .Evaluate(env, datum);
            return Evaluate(c);
        }
    }
}