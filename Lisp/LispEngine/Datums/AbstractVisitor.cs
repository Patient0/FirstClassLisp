using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    abstract class AbstractVisitor<T> : DatumVisitor<T>
    {
        public abstract T fail(Datum d);

        public virtual T visit(Pair p)
        {
            return fail(p);
        }

        public virtual T visit(Atom a)
        {
            return fail(a);
        }

        public virtual T visit(Symbol s)
        {
            return fail(s);
        }

        public virtual T visit(StackFunction s)
        {
            return fail(s);
        }

        public virtual T visit(FExpression s)
        {
            return fail(s);
        }

        public virtual T visit(Null n)
        {
            return fail(n);
        }
    }
}
