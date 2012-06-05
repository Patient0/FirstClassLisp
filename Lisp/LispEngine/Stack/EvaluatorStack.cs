using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Stack
{
    interface EvaluatorStack
    {
        void PushTask(Task task);
        Task PopTask();
        void PushResult(Datum d);
        Datum PopResult();
    }
}
