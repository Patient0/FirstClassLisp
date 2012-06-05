using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Stack
{
    interface Task
    {
        void Perform(EvaluatorStack stack);
    }
}
