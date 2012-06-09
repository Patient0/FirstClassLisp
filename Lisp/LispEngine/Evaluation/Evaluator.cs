using System;
using System.Collections.Generic;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public class Evaluator
    {
        public Datum Evaluate(Environment env, Datum datum)
        {
            var c = StackContinuation.Empty
                .PushTask(null)
                .PushResult(null)
                .Evaluate(env, datum);
            try
            {
                while (c.Task != null)
                    c = c.Task.Perform(c.PopTask());
                c = c.PopTask();
                var result = c.Result;
                c = c.PopResult();
                if (c.Result != null)
                    throw new Exception(string.Format("Additional '{0}' on result stack", c.Result));
                return result;
            }
            catch(EvaluationException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw c.error(ex, "EvaluationError", ex.Message);
            }
        }
    }
}
