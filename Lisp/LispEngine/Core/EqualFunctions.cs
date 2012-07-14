using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    class EqualFunctions : BinaryFunction
    {
        public static readonly StackFunction Eq = new EqualFunctions(true).ToStack();
        public static readonly StackFunction Equal = new EqualFunctions(false).ToStack();
        private readonly bool shallow;
        public EqualFunctions(bool shallow)
        {
            this.shallow = shallow;
        }
        protected override Datum eval(Datum arg1, Datum arg2)
        {
            if (shallow &&
                ((arg1 as Pair) != null ||
                 (arg2 as Pair) != null))
                return DatumHelpers.atom(ReferenceEquals(arg1, arg2));
            return DatumHelpers.atom(arg1.Equals(arg2));
        }

        public override string ToString()
        {
            return shallow ? ",eq?" : ",equal?";
        }
    }
}
