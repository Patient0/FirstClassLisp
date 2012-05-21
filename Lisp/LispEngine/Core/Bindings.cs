using System.Collections.Generic;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    /**
     * Encapsulates the arg list of a lambda expression
     */
    interface Bindings
    {
        Environment apply(Environment to, Datum args);
    }
}
