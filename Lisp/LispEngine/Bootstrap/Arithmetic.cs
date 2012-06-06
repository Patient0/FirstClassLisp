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
        private delegate int Op(int x, int y);

        class Operation : Function
        {
            private readonly Op op;
            public Operation(Op op)
            {
                this.op = op;
            }
            public Datum Evaluate(Datum args)
            {
                return atom(enumerateInts(args).Aggregate((x,y) => op(x,y)));
            }
        }

        private static StackFunction makeOperation(Op op)
        {
            return new Operation(op).ToStack();
        }

        public static ImmutableEnvironment Extend(ImmutableEnvironment env)
        {
            return env
                .Extend("+", makeOperation((x, y) => x + y))
                .Extend("-", makeOperation((x, y) => x - y))
                .Extend("*", makeOperation((x, y) => x * y))
                .Extend("/", makeOperation((x, y) => x / y));
        }
    }
}
