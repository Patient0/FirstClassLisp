using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Bootstrap
{
    class SymbolFunctions
    {
        class SymbolToString : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return DatumHelpers.getIdentifier(arg).ToAtom();
            }
        }

        class StringToSymbol : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return DatumHelpers.symbol(DatumHelpers.castString(arg));
            }
        }

        class GenSym : Function
        {
            private int counter = 0;
            public Datum Evaluate(Datum args)
            {
                if (!DatumHelpers.nil.Equals(args))
                    throw DatumHelpers.error("gensym accepts no arguments");
                return DatumHelpers.symbol(string.Format("generated-{0}", ++counter));
            }
        }

        public static Environment Extend(Environment env)
        {
            env = env.Extend("symbol->string", new SymbolToString().ToStack());
            env = env.Extend("string->symbol", new StringToSymbol().ToStack());
            env = env.Extend("gensym", new GenSym().ToStack());
            return env;
        }
    }
}
