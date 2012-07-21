using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Bootstrap
{
    class Arithmetic : DatumHelpers
    {
        private delegate object Op(int x, int y);

        class Operation : BinaryFunction
        {
            private readonly string name;
            private readonly Op op;
            public Operation(string name, Op op)
            {
                this.name = name;
                this.op = op;
            }
            protected override Datum eval(Datum arg1, Datum arg2)
            {
                return atom(op(castInt(arg1), castInt(arg2)));
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
                .Extend("*", makeOperation("*", (x, y) => x*y))
                .Extend("/", makeOperation("/", (x, y) => x/y))
                .Extend("<", makeOperation("<", (x, y) => x < y))
                .Extend(">", makeOperation(">", (x, y) => x > y))
                .Extend("bit-and", makeOperation("bit-and", (x , y) => x & y))
                .Extend("bit-or", makeOperation("bit-or", (x, y) => x | y))
                .Extend("bit-shift", makeOperation("bit-shift", (x, y) => x << y));
        }
    }
}
