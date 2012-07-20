using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;
using Environment = LispEngine.Evaluation.Environment;

namespace LispEngine.Core
{
    internal sealed class Lambda : AbstractFExpression
    {
        public static readonly FExpression Instance = new Lambda();

        private Lambda()
        {
        }

        private sealed class ArgBody
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

            public override string ToString()
            {
                return string.Format("{0} {1}", argDatum, body);
            }
        }

        private sealed class Closure : AbstractStackFunction
        {
            private readonly Environment env;
            private readonly IEnumerable<ArgBody> argBodies;
            public Closure(Environment env, IEnumerable<ArgBody> argBodies)
            {
                this.env = env;
                this.argBodies = argBodies;
            }

            Exception bindError(Datum args)
            {
                return DatumHelpers.error("Could not bind '{0}' to '{1}'", argList(), args);
            }

            private string argList()
            {
                return string.Join(" or ", argBodies.Select(a => a.argDatum.ToString()));
            }

            public override string ToString()
            {
                var name = env.ReverseLookup(this);
                if (name != null)
                    return name;
                return string.Format("(lambda {0})", string.Join(" ", argBodies.Select(x => x.ToString()).ToArray()));
            }

            public override Continuation Evaluate(Continuation c, Datum args)
            {
                foreach (var ab in argBodies)
                {
                    var closureEnv = ab.binding(env, args);
                    if (closureEnv == null) continue;
                    return c.Evaluate(closureEnv, ab.body);
                }
                throw bindError(args);
            }
        }

        private static Datum evaluate(Continuation c, Environment env, Datum args)
        {
            var macroArgs = args.ToArray();
            if (macroArgs.Length % 2 != 0)
                throw c.error("Invalid macro syntax for lambda. Argument count for '{0}' is not even ({1}). Syntax is (lambda [args body]+)", args, macroArgs.Length);

            var argBodies = new List<ArgBody>();
            for (var i = 0; i < macroArgs.Length; i += 2)
            {
                var closureArgs = macroArgs[i];
                var body = macroArgs[i + 1];
                argBodies.Add(new ArgBody(closureArgs, body));
            }
            return new Closure(env, argBodies);            
        }

        public override Continuation Evaluate(Continuation c, Environment env, Datum args)
        {
            return c.PushResult(evaluate(c, env, args));
        }
    }
}
