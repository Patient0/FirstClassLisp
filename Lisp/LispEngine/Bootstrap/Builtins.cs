using System;
using System.IO;
using System.Reflection;
using LispEngine.Core;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
using LispEngine.ReflectionBinding;
using LispEngine.Util;

namespace LispEngine.Bootstrap
{
    /**
     * Here we define everything that can be defined in Lisp itself
     * from the Core. i.e. everything that is 'standard' but which
     * can be defined once the Core is defined.
     */
    public sealed class Builtins
    {
        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            env = Arithmetic.Extend(env);
            env.Define("append", Append.Instance);
            env = SymbolFunctions.Extend(env);
            ResourceLoader.ExecuteResource(env, "LispEngine.Bootstrap.Builtins.lisp");
            ResourceLoader.ExecuteResource(env, "LispEngine.Bootstrap.Library.lisp");
            env = Reader.AddTo(env);
            return env;
        }
    }
}
