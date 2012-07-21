using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    class VectorFunctions : DatumHelpers
    {
        private static readonly Datum zero = DatumHelpers.atom(0);
        class MakeVector : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                if (argArray.Length != 1 && argArray.Length != 2)
                    throw error("1 or 2 arguments for make-vector");
                var size = argArray[0].CastInt();
                var initial = argArray.Length == 1 ? zero : argArray[1];
                var array = new Datum[size];
                for (int i = 0; i < size; ++i)
                    array[i] = initial;
                return vector(array);
            }

            public override string ToString()
            {
                return ",make-vector";
            }
        }

        class VectorConstructor : Function
        {
            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                return vector(argArray);
            }

            public override string ToString()
            {
                return ",vector";
            }
        }

        private static Datum isVector(Datum d)
        {
            return (d is Vector).ToAtom();
        }

        // Seems crazy that this method does exist somewhere
        // in .Net already... Perhaps it does. I could not find it.
        private static T[] CreateCopy<T>(T[] elements)
        {
            var copy = new T[elements.Length];
            Array.Copy(elements, copy, elements.Length);
            return copy;
        }

        private static Datum vectorSet(Datum v, Datum index, Datum value)
        {
            return castVector(v).Elements[index.CastInt()] = value;
        }

        private static Datum vectorCopy(Datum d)
        {
            return vector(CreateCopy(castVector(d).Elements));
        }

        private static Datum vectorLength(Datum d)
        {
            return castVector(d).Elements.Length.ToAtom();
        }

        private static Datum vectorRef(Datum d, Datum index)
        {
            return castVector(d).Elements[index.CastInt()];
        }

        public static Environment AddTo(Environment env)
        {
            env = env.Extend("make-vector", new MakeVector().ToStack());
            env = env.Extend("vector", new VectorConstructor().ToStack());
            env = env.Extend("vector?", DelegateFunctions.MakeDatumFunction(isVector, ",vector?"));
            env = env.Extend("vector-copy", DelegateFunctions.MakeDatumFunction(vectorCopy, ",vector-copy"));
            env = env.Extend("vector-set!", DelegateFunctions.MakeDatumFunction(vectorSet, ",vector-set!"));
            env = env.Extend("vector-length", DelegateFunctions.MakeDatumFunction(vectorLength, ",vector-length"));
            env = env.Extend("vector-ref", DelegateFunctions.MakeDatumFunction(vectorRef, ",vector-ref"));
            return env;
        }
    }
}
