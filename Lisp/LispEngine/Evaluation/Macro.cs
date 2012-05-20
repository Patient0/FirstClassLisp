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
     * 
     * As far as the C# code is concerned, a Macro is as powerful as
     * an F-expression in that it can choose to evaluate the arguments
     * as well as just re-arranging them.
     * 
     * So "lambda" and "if" take advantage of this.
     * 
     * In terms of user-defined macros... this will depend on whether
     * the "Environment" will be passed as a parameter to the
     * Macro implementation.
     * 
     * We might want to distinguish the two. (i.e have Macro and FExpression
     * be different interfaces - only FExpression is passed Environment).
     */
    interface Macro : Datum
    {
        Datum Expand(Evaluator evaluator, Environment env, Datum args);
    }
}
