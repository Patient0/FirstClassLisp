using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LispEngine.Datums
{
    // Used to "clone" a Datum with the intent of removing any
    // graph-like structure. See symbol-lookup-cache unit test
    // for reason why this is necessary.
    class GraphToTree : AbstractVisitor<Datum>
    {
        public override Datum visit(Pair p)
        {
            return new Pair(p.First.accept(this), p.Second.accept(this));
        }
        public override Datum visit(Symbol s)
        {
            return s.clone();
        }
        public override Datum defaultCase(Datum d)
        {
            return d;
        }

        public static AbstractVisitor<Datum> Instance = new GraphToTree();
    }
}
