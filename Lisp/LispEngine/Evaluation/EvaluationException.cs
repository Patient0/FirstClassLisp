using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Evaluation
{
    public class EvaluationException : Exception
    {
        private readonly Continuation c;
        public EvaluationException(string msg, Continuation c, Exception cause)
            : base(msg, cause)
        {
            this.c = c;
        }

        public string LispTrace
        {
            get
            {
                return string.Format("Continuation:\n{0}\n", c.GetStackTrace());
            }
        }

        /**
         * Where the exception happened.
         */
        public Continuation Continuation
        {
            get { return c; }
        }
    }
}
