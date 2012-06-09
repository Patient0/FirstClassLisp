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

        public override string StackTrace
        {
            get
            {
                return string.Format("LispTrace:\n{0}\n{1}",
                                     c.GetStackTrace(), base.StackTrace);
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
