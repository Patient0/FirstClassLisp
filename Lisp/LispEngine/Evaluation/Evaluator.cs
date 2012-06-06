using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    public sealed class Evaluator : DatumHelpers
    {
        public static FExpression toFExpression(Datum d)
        {
            var fexpr = d as FExpression;
            if (fexpr != null)
                return fexpr;
            var function = d as StackFunction;
            if (function != null)
                return new FunctionExpression(function);
            throw error("'{0}' is not callable", d);
        }
    }
}
