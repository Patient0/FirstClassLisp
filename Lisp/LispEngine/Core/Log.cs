using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class Log : Function
    {
        public static readonly StackFunction Instance = new Log().ToStack();
        public Datum Evaluate(Datum args)
        {
            foreach(var arg in DatumHelpers.enumerate(args))
                Console.WriteLine("{0}", arg);
            return args;
        }
    }
}
