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
            private readonly string name;
            private readonly Op op;
            public Operation(string name, Op op)
            {
                this.name = name;
                this.op = op;
            }
            public Datum Evaluate(Datum args)
            {
                return atom(enumerateInts(args).Aggregate((x,y) => op(x,y)));
            }
            public override string ToString()
            {
                return name;
            }
        }

        private static StackFunction makeOperation(string name, Op op)
        {
           return new Operation(name, op).ToStack();
        }

        public static IEnvironment Extend(IEnvironment env)
        {
            return env
                .Extend("+", makeOperation("+", (x, y) => x + y))
                .Extend("-", makeOperation("-", (x, y) => x - y))
                .Extend("*", makeOperation("*", (x, y) => x * y))
                .Extend("/", makeOperation("/", (x, y) => x / y));
        }
    }
}
