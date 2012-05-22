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
        class Builder
        {
            private readonly Evaluator bootstrapper = new Evaluator();
            public Environment env;

            public Builder(Environment env)
            {
                this.env = env;
            }

            private Datum evaluate(string sexp)
            {
                var parser = new Parser(Scanner.Create(sexp));
                var datum = parser.parse();
                return bootstrapper.Evaluate(env, datum);
            }

            public void add(string symbol, string sexp)
            {
                env = env.Extend(symbol, evaluate(sexp));
            }
        }

        public static Environment AddTo(Environment env)
        {
            var b = new Builder(env);
            b.add("list", "(lambda x x)");
            b.add("car", "(lambda (list) (apply (lambda (x . y) x) list))");
            b.add("cdr", "(lambda (list) (apply (lambda (x . y) y) list))");
            b.add("letdef", "(lambda (var value body) (list (list lambda (list var) body) value))");
            b.add("let", "(macro letdef)");
            return b.env;
        }
    }
}
