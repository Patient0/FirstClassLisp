using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    using FrameBinder = Func<Datum, IStack<LexicalEnvironment.Binding>>;

    internal sealed class Lambda : AbstractFExpression
    {
        public static readonly FExpression Instance = new Lambda();

        private Lambda()
        {
        }

        private sealed class ArgBody
        {
            public readonly Datum argDatum;
            public readonly FrameBinder binding;
            public readonly Datum body;
            public ArgBody(Datum argDatum, Datum body)
            {
                this.argDatum = argDatum;
                this.binding = LexicalBinder.Create(argDatum);
                this.body = body;
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", argDatum, body);
            }
        }

        private sealed class Closure : AbstractStackFunction
        {
            private readonly LexicalEnvironment env;
            private readonly IEnumerable<ArgBody> argBodies;
            public Closure(LexicalEnvironment env, IEnumerable<ArgBody> argBodies)
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
                return string.Format("(lambda {0})", string.Join(" ", argBodies.Select(x => x.ToString()).ToArray()));
            }

            public override Continuation Evaluate(Continuation c, Datum args)
            {
                foreach (var ab in argBodies)
                {
                    var frameBindings = ab.binding(args);
                    if (frameBindings == null)
                        continue;
                    var closureEnv = env.NewFrame(frameBindings);
                    return c.Evaluate(closureEnv, ab.body);
                }
                throw bindError(args);
            }
        }

        private static Datum evaluate(Continuation c, LexicalEnvironment env, Datum args)
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

        public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
        {
            return c.PushResult(evaluate(c, env, args));
        }
    }
}
