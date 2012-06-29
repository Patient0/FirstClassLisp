using System;
using System.IO;
using System.Reflection;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.Util;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Bootstrap
{
    /**
     * Here we define everything that can be defined in Lisp itself
     * from the Core. i.e. everything that is 'standard' but which
     * can be defined once the Core is defined.
     */
    public sealed class Builtins
    {
        public static Environment AddTo(Environment env)
        {
            var evaluator = new Evaluator();
            env = Arithmetic.Extend(env).ToMutable();
            env = env.Extend("append", Append.Instance);
            env = SymbolFunctions.Extend(env);
            foreach (var d in ResourceLoader.ReadDatums("LispEngine.Bootstrap.Builtins.lisp"))
                evaluator.Evaluate(env, d);
            return env;
        }
    }
}
