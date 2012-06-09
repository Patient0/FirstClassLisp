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

        public override string ToString()
        {
            return string.Format("EvaluationException!\n{0}\n{1}",
                                 base.ToString(),
                                 c.GetStackTrace());
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
