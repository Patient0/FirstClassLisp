using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class StackEvaluator
    {

        public Datum Evaluate(Environment env, Datum datum)
        {
            Continuation c = StackContinuation.Empty;
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
