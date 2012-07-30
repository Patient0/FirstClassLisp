using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LispEngine.Datums;
using LispEngine.Evaluation;

namespace LispEngine.Core
{
    /**
     * Turns a function into something that
     * can be used as a macro.
     */
    internal class Macro : DatumHelpers, Function
    {
        public static readonly StackFunction Instance = new Macro().ToStack();
        public static readonly StackFunction Unmacro = new UnMacro().ToStack();

        public static LexicalEnvironment AddTo(LexicalEnvironment env)
        {
            env.Define("unmacro", Unmacro);
            env.Define("macro", Instance);
            return env;
        }

        private sealed class UnMacro : UnaryFunction
        {
            protected override Datum eval(Datum arg)
            {
                var macro = arg as MacroClosure;
                return macro == null ? nil : macro.Function;
            }
        }

        private sealed class EvaluateExpansion : Task
        {
            private readonly MacroClosure macro;
            private readonly Pair macroDatum;

            public EvaluateExpansion(MacroClosure macro, Pair macroDatum)
            {
                this.macro = macro;
                this.macroDatum = macroDatum;
            }

            public Continuation Perform(Continuation c)
            {
                // The result datum may be a graph. This makes certain
                // optimizations risky.
                var expansion = c.Result;
                if(macroDatum != null)
                {
                    // Cache macro expansions. In the extremely
                    // common case of the same macro being used on the
                    // same Datum, re-use the expansion.
                    macroDatum.Cache = cons(macro, expansion);
                }
                return c.PopResult().PopEnv().Evaluate(c.Env, expansion);
            }
        }

        private class MacroClosure : AbstractFExpression
        {
            private readonly StackFunction argFunction;

            public StackFunction Function
            {
                get { return argFunction; }
            }

            public MacroClosure(StackFunction argFunction)
            {
                this.argFunction = argFunction;
            }

            public override Continuation Evaluate(Continuation c, LexicalEnvironment env, Datum args)
            {
                var p = args as Pair;
                c = c.PushEnv(env).PushTask(new EvaluateExpansion(this, p));
                // Optimization - if this macro has been expanded on this Datum before,
                // use the same expansion.
                // See "macro-cache-in-datum" unit test for a demonstration of why
                // we need to check against "this" also.
                if(p != null)
                {
                    var cachedPair = p.Cache as Pair;
                    if(cachedPair != null && ReferenceEquals(cachedPair.First, this))
                        return c.PushResult(cachedPair.Second);
                }
                c.Statistics.Expansions++;
                return argFunction.Evaluate(c, args);
            }

            public override string ToString()
            {
                return string.Format("(,macro {0})", argFunction);
            }
        }

        public static FExpression ToMacro(StackFunction function)
        {
            return new MacroClosure(function);
        }

        public Datum Evaluate(Datum args)
        {
            var function = UnaryFunction.GetSingle(args) as StackFunction;
            if (function == null)
                throw error("Expected function argument");
            return ToMacro(function);
        }
    }
}
