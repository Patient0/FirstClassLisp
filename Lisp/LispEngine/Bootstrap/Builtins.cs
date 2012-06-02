using System;
using System.IO;
using System.Reflection;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Lexing;
using LispEngine.Parsing;
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
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("LispEngine.Bootstrap.Builtins.lisp");
            if (stream == null)
                throw new Exception("Unable to find Builtins.lisp embedded resource");
            var s = new Scanner(new StreamReader(stream)) { Filename = "Builtins.lisp" };
            var p = new Parser(s);
            Datum d = null;
            var evaluator = new Evaluator();
            while((d = p.parse()) != null)
            {
                evaluator.Evaluate(env, d);
            }
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
