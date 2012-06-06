using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Stack;

namespace LispEngine.Evaluation
{
    interface StackFunction : Datum
    {
        void Evaluate(EvaluatorStack s, Datum args);
    }
}
