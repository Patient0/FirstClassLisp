using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;

namespace LispEngine.Evaluation
{
    /**
     * Implementations of StackFunction based on Func delegates.
     */
    class DelegateFunctions
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
                if (!(input is T))
                    throw DatumHelpers.error("Expected '{0}' to be of type '{1}'", arg, typeof(T).Name);
                return funcDelegate((T)input).ToAtom();
            }

            public override string ToString()
            {
                return name;
            }
        }


        class TernaryDatumDelegateFunction : Function
        {
            private readonly string name;
            private readonly Func<Datum, Datum, Datum, Datum> funcDelegate;
            public TernaryDatumDelegateFunction(string name, Func<Datum, Datum, Datum, Datum> funcDelegate)
            {
                this.name = name;
                this.funcDelegate = funcDelegate;
            }


            public override string ToString()
            {
                return name;
            }

            public Datum Evaluate(Datum args)
            {
                var argArray = args.ToArray();
                if (argArray.Length != 3)
                    throw DatumHelpers.error("{0}: 3 arguments expected, got {1}", name, argArray.Length);
                return funcDelegate(argArray[0], argArray[1], argArray[2]);
            }
        }

        class BinaryDatumDelegateFunction : BinaryFunction
        {
            private readonly string name;
            private readonly Func<Datum, Datum, Datum> funcDelegate;
            public BinaryDatumDelegateFunction(string name, Func<Datum, Datum, Datum> funcDelegate)
            {
                this.name = name;
                this.funcDelegate = funcDelegate;
            }

            protected override Datum eval(Datum arg1, Datum arg2)
            {
                return funcDelegate(arg1, arg2);
            }

            public override string ToString()
            {
                return name;
            }
        }

        class UnaryDatumDelegateFunction : UnaryFunction
        {
            private readonly string name;
            private readonly Func<Datum, Datum> funcDelegate;
            public UnaryDatumDelegateFunction(string name, Func<Datum, Datum> funcDelegate)
            {
                this.name = name;
                this.funcDelegate = funcDelegate;
            }

            protected override Datum eval(Datum arg)
            {
                return funcDelegate(arg);
            }
            public override string ToString()
            {
                return name;
            }
        }

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

        public static Datum MakeDatumFunction(Func<Datum, Datum> func, string name)
        {
            return new UnaryDatumDelegateFunction(name, func).ToStack();
        }

        public static Datum MakeDatumFunction(Func<Datum, Datum, Datum> func, string name)
        {
            return new BinaryDatumDelegateFunction(name, func).ToStack();
        }

        public static Datum MakeDatumFunction(Func<Datum, Datum, Datum, Datum> func, string name)
        {
            return new TernaryDatumDelegateFunction(name, func).ToStack();
        }


        public static Datum MakeFunction<TResult>(Func<TResult> func, string name)
        {
            return new AccessorFunction<TResult>(name, func).ToStack();
        }

        public static Datum MakeFunction<T, TResult>(Func<T, TResult> func, string name)
        {
            return new UnaryDelegateFunction<T, TResult>(name, func).ToStack();
        }
    }
}
