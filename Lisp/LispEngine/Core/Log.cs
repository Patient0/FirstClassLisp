using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Log : UnaryFunction
    {
        public static readonly StackFunction Instance = new Log().ToStack();
        protected override Datum eval(Datum arg)
        {
            Console.WriteLine("{0}", arg);
            return arg;
        }
    }
}
