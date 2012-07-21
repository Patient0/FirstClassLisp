using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class VectorFunctions
    {
        private static readonly Datum zero = DatumHelpers.atom(0);
        class MakeVector : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                if (argArray.Length != 1 && argArray.Length != 2)
                    throw DatumHelpers.error("1 or 2 arguments for make-vector");
                var size = argArray[0].CastInt();
                var initial = argArray.Length == 1 ? zero : argArray[1];
                var array = new Datum[size];
                for (int i = 0; i < size; ++i)
                    array[i] = initial;
                return DatumHelpers.vector(array);
            }

            public override string ToString()
            {
                return ",make-vector";
            }
        }

        public static Environment AddTo(Environment env)
        {
            env = env.Extend("make-vector", new MakeVector().ToStack());
            return env;
        }
    }
}
