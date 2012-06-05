using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using LispEngine.Stack;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    internal sealed class Lambda : DatumHelpers, FExpression
    {
        public static readonly FExpression Instance = new Lambda();

        private Lambda()
        {
        }

        private class ArgBody
        {
            public readonly Datum argDatum;
            public readonly Bindings binding;
            public readonly Datum body;
            public ArgBody(Datum argDatum, Datum body)
            {
                this.argDatum = argDatum;
                this.binding = BindingTypes.parse(argDatum);
                this.body = body;
            }
        }

        private sealed class Closure : Function
        {
            private readonly Environment env;
            private readonly IEnumerable<ArgBody> argBodies;
            public Closure(Environment env, IEnumerable<ArgBody> argBodies)
            {
                this.env = env;
                this.argBodies = argBodies;
            }
            public Datum Evaluate(Evaluator evaluator, Datum args)
            {
                foreach(var ab in argBodies)
                {
                    var closureEnv = ab.binding.apply(env, args);
                    if (closureEnv != null)
                        return evaluator.Evaluate(closureEnv, ab.body);
                }
                throw error("Could not bind '{0}' to '{1}'", argList(), args);
            }

            private string argList()
            {
                return string.Join(" or ", argBodies.Select(a => a.argDatum.ToString()));
            }
        }

        private static Datum evaluate(Environment env, Datum args)
        {
            var macroArgs = enumerate(args).ToArray();
            if (macroArgs.Length % 2 != 0)
                throw error("Invalid macro syntax for lambda. Argument count '{0}' is not even. Syntax is (lambda [args body]+)", macroArgs.Length);

            var argBodies = new List<ArgBody>();
            for (var i = 0; i < macroArgs.Length; i += 2)
            {
                var closureArgs = macroArgs[i];
                var body = macroArgs[i + 1];
                argBodies.Add(new ArgBody(closureArgs, body));
            }
            return new Closure(env, argBodies);            
        }

        public Datum Evaluate(Evaluator evaluator, Environment env, Datum args)
        {
            return evaluate(env, args);
        }

        public void Evaluate(EvaluatorStack evaluator, Environment env, Datum args)
        {
            evaluator.PushResult(evaluate(env, args));
        }
    }
}
