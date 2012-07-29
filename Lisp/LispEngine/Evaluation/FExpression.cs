using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    /**
     * An FExpression is passed the un-evaluated arguments.
     * It also has the current environment. It can therefore:
     * a) Choose to evaluate which (if any) of the input arguments it likes.
     * b) Choose to evaluate the result
     * The "first-class" nature of our lisp interpreter relies on the fact
     * Pair.First in any compound expression implements (or can be made to implement)
     * FExpression
     */
    public interface FExpression : Datum
    {
        Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args);
    }
}
