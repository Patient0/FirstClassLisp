using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public interface StackFunction : Datum
    {
        Continuation Evaluate(Continuation s, Datum args);
    }
}
