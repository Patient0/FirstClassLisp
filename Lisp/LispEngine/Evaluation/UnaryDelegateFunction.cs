using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class UnaryDelegateFunction<T, TResult> : UnaryFunction
    {
        private readonly string name;
        private readonly Func<T, TResult> funcDelegate;
        public UnaryDelegateFunction(string name, Func<T, TResult> funcDelegate)
        {
            this.name = name;
            this.funcDelegate = funcDelegate;
        }

        protected override Datum eval(Datum arg)
        {
            var input = arg.CastObject();
            if(!(input is T))
                throw DatumHelpers.error("Expected '{0}' to be of type '{1}'", arg, typeof(T).Name);
            return funcDelegate((T)input).ToAtom();
        }

        public override string ToString()
        {
            return name;
        }
    }
}
