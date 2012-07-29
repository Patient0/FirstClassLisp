using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Bootstrap
{
    class SymbolFunctions
    {
        class SymbolToString : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                return arg.CastSymbol().Identifier.ToAtom();
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
            public Datum Evaluate(Datum args)
            {
                if (!DatumHelpers.nil.Equals(args))
                    throw DatumHelpers.error("gensym accepts no arguments");
                return Symbol.GenUnique();
            }
        }

        public static LexicalEnvironment Extend(LexicalEnvironment env)
        {
            env.Define("symbol->string", new SymbolToString().ToStack());
            env.Define("string->symbol", new StringToSymbol().ToStack());
            env.Define("gensym", new GenSym().ToStack());
            return env;
        }
    }
}
