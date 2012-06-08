using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    abstract class AbstractStackFunction : StackFunction
    {
        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public abstract Continuation Evaluate(Continuation c, Datum args);
    }
}
