using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    public class EvaluationException : Exception
    {
        public EvaluationException(Continuation continuation, Exception ex)
            : base("Evaluation failed", ex)
        {
            this.Continuation = continuation;
        }

        public Continuation Continuation { get; private set; }

        public override string StackTrace
        {
            get {
                var sb = new StringBuilder();
                sb.Append("Tasks:\n");
                var c = Continuation;
                while(c.Task != null)
                {
                    sb.Append(c.Task.ToString());
                    sb.Append("\n");
                    c = c.PopTask();
                }
                return sb.ToString();
            }
        }
    }
}
