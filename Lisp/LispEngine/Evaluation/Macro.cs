using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    /**
     * Although there is no way to "parse" to get a Macro
     * we extend Datum as in one sense it is one of the fundamental
     * types
     */
    interface Macro : Datum
    {
        Datum Expand(Evaluator evaluator, Environment env, Datum args);
    }
}
