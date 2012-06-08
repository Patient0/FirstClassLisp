using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    // A Function that can be implemented completely outside of the interpreter.
    // Used for defining builtins, simple arithmetic, etc.
    // An "Function" can be converted into a StackFunction, suitable for
    // use in the interpreter, by using the "ToStack" extension method.
    public interface Function
    {
        Datum Evaluate(Datum args);
    }
}
