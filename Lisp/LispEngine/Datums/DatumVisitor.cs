using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Evaluation;

namespace LispEngine.Datums
{
    // The "visitor" pattern provides a raw appromiximation of
    // pattern-matching in functional languages.
    // It's useful in our case because the fundamental subclasses
    // of "Datum" will stay quite fixed but there are many cases
    // when we need to execute different behaviour based on the
    // type of the Datum.
    public interface DatumVisitor<out T>
    {
        T visit(Pair p);
        T visit(Atom a);
        T visit(Symbol s);
        T visit(StackFunction s);
        T visit(FExpression s);
        T visit(Null n);
        T visit(Vector v);
    }
}
