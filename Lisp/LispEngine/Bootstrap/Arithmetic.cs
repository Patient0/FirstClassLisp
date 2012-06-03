using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Bootstrap
{
    class Arithmetic : DatumHelpers
    {
        class Add : Function
        {
            public Datum Evaluate(Evaluator evaluator, Datum args)
            {
                return atom(enumerateInts(args).Aggregate(0, (x, y) => x + y));
            }
        }

        class Subtract : Function
        {
            public Datum Evaluate(Evaluator evaluator, Datum args)
            {
                return atom(enumerateInts(args).Aggregate((x, y) => x - y));
            }         
        }

        public static ImmutableEnvironment Extend(ImmutableEnvironment env)
        {
            return env
                .Extend("+", new Add())
                .Extend("-", new Subtract());
        }
    }
}
