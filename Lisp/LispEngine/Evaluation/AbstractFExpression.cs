﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Stack;

namespace LispEngine.Evaluation
{
    abstract class AbstractFExpression : FExpression
    {
        public T accept<T>(DatumVisitor<T> visitor)
        {
            return visitor.visit(this);
        }

        public abstract void Evaluate(EvaluatorStack evaluator, Environment env, Datum args);
    }
}
