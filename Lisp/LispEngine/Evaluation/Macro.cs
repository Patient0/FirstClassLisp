using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    interface Macro
    {
        Datum Expand(Evaluator evaluator, Environment env, Datum args);
    }
}
