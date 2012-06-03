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
            foreach (var d in ResourceLoader.ReadDatums("LispEngine.Bootstrap.Builtins.lisp"))
                evaluator.Evaluate(env, d);
            env = env.Extend("append", Append.Instance);
            // For now, implement Quasiquote in C# - will translate
            // into pure Lisp later hopefully.
            env = env.Extend("quasiquote", Macro.ToMacro(new Quasiquote(env)));

            return env;
            /*
            var parsed = p.parse();

            var b = new Builder(env);
            b.add("list", "(lambda x x)");
            b.add("car", "(lambda ((x . y)) x)");
            b.add("cdr", "(lambda ((x . y)) y)");
            b.add("nil?", "(lambda (()) #t _ #f)");
            b.add("pair?", "(lambda ((_ . _)) #t _ #f)");
            b.add("letdef", "(lambda (var value body) (list (list lambda (list var) body) value))");
            b.add("let", "(macro letdef)");
            return b.env;
             */
        }
    }
}
