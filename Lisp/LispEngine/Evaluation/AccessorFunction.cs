using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    class AccessorFunction<T> : Function
    {
        private readonly string name;
        private readonly Func<T> accessor;
        public AccessorFunction(string name, Func<T> accessor)
        {
            this.name = name;
            this.accessor = accessor;
        }
        public Datum Evaluate(Datum args)
        {
            if (!DatumHelpers.nil.Equals(args))
                throw DatumHelpers.error("No arguments expected for function '{0}'", name);
            return accessor().ToAtom();
        }

        public override string ToString()
        {
            return name;
        }
    }

}
