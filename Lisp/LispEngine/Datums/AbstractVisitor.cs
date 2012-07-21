using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    abstract class AbstractVisitor<T> : DatumVisitor<T>
    {
        public abstract T defaultCase(Datum d);

        public virtual T visit(Pair p)
        {
            return defaultCase(p);
        }

        public virtual T visit(Atom a)
        {
            return defaultCase(a);
        }

        public virtual T visit(Symbol s)
        {
            return defaultCase(s);
        }

        public virtual T visit(StackFunction s)
        {
            return defaultCase(s);
        }

        public virtual T visit(FExpression s)
        {
            return defaultCase(s);
        }

        public virtual T visit(Null n)
        {
            return defaultCase(n);
        }

        public virtual T visit(Vector v)
        {
            return defaultCase(v);
        }
    }
}
